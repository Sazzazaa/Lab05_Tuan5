using BLL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmRegisterMajor : Form
    {
        public frmRegisterMajor()
        {
            InitializeComponent();
        }
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();

        private void fillFacultyCombobox(List<Faculty> lstfacul)
        {
            cmbFaculty.DataSource = lstfacul;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }
        private void FillMajorCombobox(List<Major> lstmajor)
        {
            cmbMajor.DataSource = lstmajor;
            cmbMajor.DisplayMember = "Name";
            cmbMajor.ValueMember = "MajorID";
        }
        private void renderStudentLst(List<Student> lstStu)
        {
            dgvStudents.Rows.Clear();
            foreach (Student item in lstStu)
            {
                int index = dgvStudents.Rows.Add();
                dgvStudents.Rows[index].Cells[1].Value = item.StudentID;
                dgvStudents.Rows[index].Cells[2].Value = item.FullName;
                if (item.Faculty != null)
                {
                    dgvStudents.Rows[index].Cells[3].Value = item.Faculty.FacultyName;
                }
                dgvStudents.Rows[index].Cells[4].Value = item.AverageScore + "";
                if (item.Major != null)
                {
                    dgvStudents.Rows[index].Cells[5].Value = item.Major.Name + "";
                }
            }
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                var listMajor = majorService.getAllMajorLstByFacultyID(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);

                var listStudents = studentService.GetStudentByFacultyIDAndNoMajor(selectedFaculty.FacultyID);
                renderStudentLst(listStudents);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            List<String> result = new List<String>();
            int n = dgvStudents.Rows.Count;
            foreach (DataGridViewRow row in dgvStudents.Rows)
            {
                if (!row.IsNewRow && row.Cells[0].Selected)
                {
                    try
                    {
                        Student stuUpdate = studentService.findStudent(row.Cells[1].Value.ToString());
                        stuUpdate.MajorID = int.Parse(cmbMajor.SelectedValue.ToString());
                        studentService.InsertStudent(stuUpdate);
                    }
                    catch
                    {
                        result.Add(row.Cells[2].Value.ToString());
                    }

                }
            }
            string message = $"{n - result.Count} sinh viên đăng ký chuyên ngành thành công\n";

            if (result.Count > 0)
            {
                message += "Không đăng ký được các sinh viên sau:\n";
                foreach (string studentName in result)
                {
                    message += studentName + "\n";
                }
            }
            MessageBox.Show(message);
            var listStudents = studentService.GetStudentByFacultyIDAndNoMajor(int.Parse(cmbMajor.SelectedValue.ToString()));
            renderStudentLst(listStudents);
        }

        private void frmRegisterMajor_Load(object sender, EventArgs e)
        {
            try
            {
                var listFaculties = facultyService.getAllFaculty();
                fillFacultyCombobox(listFaculties);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Quá trình đăng ký bị lỗi " + ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbMajor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    }
