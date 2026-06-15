using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ManageCategoriesApp
{
    public sealed class ManageCategories
    {
        private static ManageCategories? instance = null;
        private static readonly object instanceLock = new object();

        private ManageCategories() { }

        public static ManageCategories Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new ManageCategories();
                        }
                    }
                }
                return instance;
            }
        }

        public List<Category> GetCategories()
        {
            List<Category> categories;
            try
            {
                using var stock = new MyStock();
                categories = stock.Categories.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCategories: " + ex.Message, ex);
            }
            return categories;
        }

        public void InsertCategory(Category category)
        {
            try
            {
                using var stock = new MyStock();
                stock.Categories.Add(category);
                stock.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in InsertCategory: " + ex.Message, ex);
            }
        }

        public void UpdateCategory(Category category)
        {
            try
            {
                using var stock = new MyStock();
                stock.Entry<Category>(category).State = EntityState.Modified;
                stock.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateCategory: " + ex.Message, ex);
            }
        }

        public void DeleteCategory(Category category)
        {
            try
            {
                using var stock = new MyStock();
                var existing = stock.Categories.Find(category.CategoryID);
                if (existing != null)
                {
                    stock.Categories.Remove(existing);
                    stock.SaveChanges();
                }
                else
                {
                    throw new Exception($"Category with ID {category.CategoryID} not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteCategory: " + ex.Message, ex);
            }
        }
    }
}
