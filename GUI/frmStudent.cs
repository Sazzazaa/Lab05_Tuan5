using BLL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmStudent : Form
    {
        public frmStudent()
        {
            InitializeComponent();
        }
        private readonly FacultyService facultyService = new FacultyService();
        private readonly StudentService studentService = new StudentService();

        public string stu_id;
        public string stu_name;
        public string stu_faculty;
        public double stu_score;

        public string pathImage;

        public Image defaultImage;


        private void renderStudentLst(List<Student> lstStu)
        {
            dgvStudents.Rows.Clear();
            foreach (Student item in lstStu)
            {
                int index = dgvStudents.Rows.Add();
                dgvStudents.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudents.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                {
                    dgvStudents.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                }
                dgvStudents.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.Major != null)
                {
                    dgvStudents.Rows[index].Cells[4].Value = item.Major.Name + "";
                }
            }
        }
        private void fillFacultyStudent(List<Faculty> lstfacul)
        {
            cmbFaculty.DataSource = lstfacul;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }

        private void refresh()
        {
            txtStudentID.Text = "";
            txtFullName.Text = "";
            cmbFaculty.SelectedIndex = 0;
            txtAverageScore.Text = "";
            picAvatar.Image = null;
        }

        private void LoadData()
        {
            var lstStuDAL = studentService.getAllStudent();
            renderStudentLst(lstStuDAL);
            lblViewTotal.Text = studentService.TotalStudent(lstStuDAL).ToString();
        }
        private bool ValidateStudentID(string id)
        {
            if (id.Length != 10)
            {
                return false;
            }
            return true;
        }
        private bool ValidateName(string name)
        {
            if (name.Length < 3 || name.Length > 100)
            {
                return false;
            }
            if (name.Any(char.IsDigit))
            {
                return false;
            }
            return true;
        }
        private bool ValidateGPA(string gpaText)
        {

            if (double.TryParse(gpaText, out double gpa))
            {
                return gpa >= 0 && gpa <= 10;
            }
            return false;
        }
        private bool checkNull(string stuID, string stuName, string stuScore)
        {
            if (string.IsNullOrEmpty(stuID) || string.IsNullOrEmpty(stuName) ||
            string.IsNullOrEmpty(stuScore))
            {
                return false;
            }
            return true;
        }

        private bool checkDulStudentID(string stuID)
        {
            for (int i = 0; i < dgvStudents.Rows.Count; i++)
            {
                if (dgvStudents.Rows[i].Cells[0].Value != null && !string.IsNullOrEmpty(dgvStudents.Rows[i].Cells[0].Value.ToString()))
                {
                    if (dgvStudents.Rows[i].Cells[0].Value.ToString() == stuID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
            try
            {

                var listStudentDAL = studentService.getAllStudent();
                var listFacultyDAL = facultyService.getAllFaculty();
                renderStudentLst(listStudentDAL);
                fillFacultyStudent(listFacultyDAL);
                picAvatar.Image = defaultImage;
                lblViewTotal.Text = studentService.TotalStudent(listStudentDAL).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAvatar_Click(object sender, EventArgs e)
        {
            String imageLocation = "";
            try
            {
                OpenFileDialog fileOpen = new OpenFileDialog();
                fileOpen.Title = "Chọn hình ảnh sinh viên";
                fileOpen.Filter = "Hình ảnh (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";

                if (fileOpen.ShowDialog() == DialogResult.OK)
                {
                    imageLocation = fileOpen.FileName;
                    picAvatar.Image = Image.FromFile(imageLocation);
                    pathImage = imageLocation;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi không thể upload ảnh!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            if (checkNull(txtStudentID.Text.Trim(), txtFullName.Text.Trim(), txtAverageScore.Text.Trim()))
            {
                if (checkDulStudentID(txtStudentID.Text))
                {
                    string id = txtStudentID.Text.Trim();
                    Student stuUpdate = studentService.findStudent(id);
                    if (stuUpdate != null)
                    {
                        stuUpdate.FullName = txtFullName.Text.Trim();
                        stuUpdate.AverageScore = double.Parse(txtAverageScore.Text);
                        stuUpdate.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());

                        if (!string.IsNullOrEmpty(pathImage) && File.Exists(pathImage))
                        {
                            string imageName = txtStudentID.Text.Trim() + Path.GetExtension(pathImage).TrimStart('.');
                            string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                            string imagePath = Path.Combine(parentDirectory, "Images");

                            if (!Directory.Exists(imagePath))
                            {
                                Directory.CreateDirectory(imagePath);
                            }

                            string imageDestinationPath = Path.Combine(imagePath, imageName);
                            File.Copy(pathImage, imageDestinationPath, true); // Sao chép ảnh

                            stuUpdate.Avatar = imageName;
                        }
                        else
                        {
                            stuUpdate.Avatar = stuUpdate.Avatar ?? "default.png"; // Ví dụ: sử dụng ảnh mặc định
                        }

                        // Cập nhật sinh viên vào hệ thống
                        studentService.InsertStudent(stuUpdate);
                        LoadData();
                        MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnDelete.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên trong hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Nếu là thêm mới sinh viên
                    if (!string.IsNullOrEmpty(pathImage) && File.Exists(pathImage))
                    {
                        // Nếu có ảnh mới
                        string imageName = txtStudentID.Text.Trim() + Path.GetExtension(pathImage).TrimStart('.');
                        string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                        string imagePath = Path.Combine(parentDirectory, "Images");

                        if (!Directory.Exists(imagePath))
                        {
                            Directory.CreateDirectory(imagePath);
                        }

                        string imageDestinationPath = Path.Combine(imagePath, imageName);
                        File.Copy(pathImage, imageDestinationPath, true); // Sao chép ảnh

                        // Tạo mới sinh viên
                        Student stu = new Student()
                        {
                            StudentID = txtStudentID.Text.Trim(),
                            FullName = txtFullName.Text.Trim(),
                            FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString()),
                            AverageScore = double.Parse(txtAverageScore.Text),
                            Avatar = imageName
                        };

                        studentService.InsertStudent(stu);
                        LoadData();
                        MessageBox.Show("Thêm dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnDelete.Enabled = true;
                    }
                    else
                    {
                        // Nếu không có ảnh mới, sử dụng ảnh mặc định
                        string defaultImageName = "default.png"; // Tên ảnh mặc định

                        // Tạo mới sinh viên với ảnh mặc định
                        Student stu = new Student()
                        {
                            StudentID = txtStudentID.Text.Trim(),
                            FullName = txtFullName.Text.Trim(),
                            FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString()),
                            AverageScore = double.Parse(txtAverageScore.Text),
                            Avatar = defaultImageName // Gán ảnh mặc định
                        };

                        studentService.InsertStudent(stu);
                        LoadData();
                        MessageBox.Show("Thêm dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnDelete.Enabled = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            btnAddOrUpdate.Enabled = false;
            if (checkNull(txtStudentID.Text.Trim(), txtFullName.Text.Trim(), txtAverageScore.Text.Trim()))
            {
                Student stuUpdate = studentService.findStudent(txtStudentID.Text);
                if (stuUpdate != null)
                {
                    if (MessageBox.Show($"Bạn muốn xóa sinh viên {txtFullName.Text}?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        studentService.DeleteStudent(txtStudentID.Text);
                        LoadData();
                        refresh();
                        MessageBox.Show("Xóa dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtStudentID.Enabled = true;
                        btnAddOrUpdate.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên trong hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var lstStudent = new List<Student>();
            if (chkUnregisterMajor.Checked)
            {
                lstStudent = studentService.GetAllHashNoMajor();
                renderStudentLst(lstStudent);
            }
            else
            {
                lstStudent = studentService.getAllStudent();
                renderStudentLst(lstStudent);
            }
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           if (e.RowIndex >= 0)
          {
              DataGridViewRow selectedRow = dgvStudents.Rows[e.RowIndex];
               txtStudentID.Text = selectedRow.Cells[0].FormattedValue.ToString();
               txtFullName.Text = selectedRow.Cells[1].FormattedValue.ToString();
              cmbFaculty.SelectedIndex = cmbFaculty.FindString(dgvStudents.Rows[e.RowIndex].Cells[2].FormattedValue.ToString());
               txtAverageScore.Text = dgvStudents.Rows[e.RowIndex].Cells[3].FormattedValue.ToString();
            string idStu = dgvStudents.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
           string imgName = studentService.findImageStudent(idStu);
//             if (!string.IsNullOrEmpty(imgName))
//              {
//                 string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
//                   string imagePath = Path.Combine(parentDirectory, "Images", imgName);
//                  picAvatar.Image = Image.FromFile(imagePath);
//              }
//               else
//               {
//
//                   picAvatar.Image = null;
//               }
            }
        }

        private void đăngKýChuyênNgànhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRegisterMajor f = new frmRegisterMajor();
            f.ShowDialog();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
