using DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class FacultyService
    {
        public List<Faculty> getAllFaculty()
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            return studentModel.Faculties.ToList();
        }
        public Faculty findFaculty(int id)
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            return studentModel.Faculties.FirstOrDefault(p => p.FacultyID == id);
        }
        public void insertFaculty(Faculty facItem)
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            studentModel.Faculties.AddOrUpdate(facItem);
            studentModel.SaveChanges();
        }
        public void updateFaculty(Faculty facItem)
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            studentModel.Faculties.AddOrUpdate(facItem);
            studentModel.SaveChanges();
        }
        public void deleteFaculty(int facID)
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            Faculty item = studentModel.Faculties.FirstOrDefault(p => p.FacultyID == facID);
            studentModel.Faculties.Remove(item);
            studentModel.SaveChanges();
        }
    }
}