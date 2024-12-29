using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MajorService
    {
        public List<Major> getAllMajorLst()
        {
            QLSVDBContext studentModel = new QLSVDBContext();
            return studentModel.Majors.ToList();
        }
        public List<Major> getAllMajorLstByFacultyID(int facultyId)
        {
            using (QLSVDBContext studentModel = new QLSVDBContext())
            {
                return studentModel.Majors
                    .Where(m => m.FacultyID == facultyId)
                    .ToList();
            }
        }
    }
}
