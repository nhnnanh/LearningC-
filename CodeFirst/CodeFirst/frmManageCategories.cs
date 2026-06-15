using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ManageCategoriesApp
{
    public partial class frmManageCategories : Form
    {
        private BindingSource source = new BindingSource();

        public frmManageCategories()
        {
            InitializeComponent();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = ManageCategories.Instance.GetCategories();

                // Clear existing bindings to prevent duplicate binding exceptions
                txtCategoryID.DataBindings.Clear();
                txtCategoryName.DataBindings.Clear();

                // Set DataSource
                source.DataSource = categories;
                dgvCategories.DataSource = source;

                // Configure bindings
                txtCategoryID.DataBindings.Add("Text", source, "CategoryID", true, DataSourceUpdateMode.OnPropertyChanged);
                txtCategoryName.DataBindings.Add("Text", source, "CategoryName", true, DataSourceUpdateMode.OnPropertyChanged);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmManageCategories_Load(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            string categoryName = txtCategoryName.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Category Name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            if (categoryName.Length > 40)
            {
                MessageBox.Show("Category Name cannot exceed 40 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            try
            {
                var category = new Category { CategoryName = categoryName };
                ManageCategories.Instance.InsertCategory(category);
                
                MessageBox.Show("Category inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to insert category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Validation
            if (!int.TryParse(txtCategoryID.Text, out int categoryId))
            {
                MessageBox.Show("Please select a valid Category to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string categoryName = txtCategoryName.Text.Trim();
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Category Name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            if (categoryName.Length > 40)
            {
                MessageBox.Show("Category Name cannot exceed 40 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            try
            {
                var category = new Category
                {
                    CategoryID = categoryId,
                    CategoryName = categoryName
                };

                ManageCategories.Instance.UpdateCategory(category);
                
                MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Validation
            if (!int.TryParse(txtCategoryID.Text, out int categoryId))
            {
                MessageBox.Show("Please select a valid Category to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete",
                                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var category = new Category { CategoryID = categoryId };
                ManageCategories.Instance.DeleteCategory(category);
                
                MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
