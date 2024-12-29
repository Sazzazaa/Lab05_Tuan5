using DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


///Mong Khang
namespace BLL
{
    public class StudentService
    {
        public List<Student> getAllStudent()
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            return studentModel.Students.ToList();
        }
        public Student findStudent(string studentId)
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            return studentModel.Students.FirstOrDefault(p => p.StudentID == studentId);
        }
        public string findImageStudent(string studentId)
        {
            QLSVDBContext studentModel = new QLSVDBContext  ();
            Student stu = studentModel.Students.FirstOrDefault(p => p.StudentID == studentId);
            if (stu != null)
            {
                return stu.Avatar;
            }
            return null;
        }
        public List<Student> GetStudentByFacultyIDAndNoMajor(int facultyId)
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            return studentModel.Students.Where(p => p.MajorID == null && p.FacultyID == facultyId).ToList();
        }
        public List<Student> GetAllHashNoMajor()
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            return studentModel.Students.Where(p => p.MajorID == null).ToList();
        }
        public void InsertStudent(Student stu)
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            studentModel.Students.AddOrUpdate(stu);
            studentModel.SaveChanges();
        }
        public List<Student> getAllStudentsByFacultyID(int facultyId)
        {
            using (QLSVDBContext studentModel = new QLSVDBContext())
            {
                return studentModel.Students
                    .Where(m => m.FacultyID == facultyId)
                    .ToList();
            }
        }
        public int TotalStudent(List<Student> lstStudents)
        {
            return lstStudents.Count;
        }
        public void DeleteStudent(string studentId)
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            Student student = studentModel.Students.FirstOrDefault(p => p.StudentID == studentId);
            studentModel.Students.Remove(student);
            studentModel.SaveChanges();
        }
    }
}
