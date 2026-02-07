using ManageLMS.DTO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageLMS.BLL.Manager.CategoryManager
{
    class CategoryManager : BaseManager
    {
        public long CreateSemesterCategory(string name, string idNumber)
        {
            return _cateRepo.CreateCategory(name, idNumber, name);
        }
        public Category GetCategoryByIdNumber(string idnumber)
        {
            return _cateRepo.GetCategoryByIdNumber(idnumber);
        }
        public long CreateCategory(string name, string idNumber, string description, int parentId)
        {
            
            return _cateRepo.CreateCategory(name, idNumber, description, parentId);
        }
    }
}
