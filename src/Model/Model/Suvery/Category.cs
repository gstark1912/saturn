using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Suvery
{
    public class Category
    {
        public Category() 
        {
            this.subCategories = new List<Category>();
            this.CreatedAt = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CompaniesDirectory { get; set; }
        public string ConsultantsDirectory { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Category parentCategory { get; set; }
        public virtual ICollection<Category> subCategories { get; set; }

        public String toJson()
        {
            return this.toCategoryJson().toJson();
        }

        public String getFullName()
        {
            if(this.parentCategory != null) {
                return this.parentCategory.getFullName() + " - " + this.Name;
            } else {
                return this.Name;
            }
        }

        private CategoryJson toCategoryJson()
        {
            CategoryJson categoryJson = new CategoryJson();
            categoryJson.Id = this.Id.ToString();
            categoryJson.Text = this.Name;
            categoryJson.Childs = arrayToCategoryJson(this.subCategories);
            return categoryJson;
        }

        private ICollection<CategoryJson> arrayToCategoryJson(ICollection<Category> categories)
        {
            ICollection<CategoryJson> result = new List<CategoryJson>();
            foreach(Category category in categories)
            {
                result.Add(category.toCategoryJson());
            }
            return result;
        }

        private class CategoryJson
        {
            public String Id { get; set; }
            public String Text { get; set; }
            public ICollection<CategoryJson> Childs { get; set; }

            public CategoryJson()
            {
                this.Childs = new List<CategoryJson>();
            }

            public String toJson()
            {
                String result = "{";
                result = result + "\"id\": \"" + this.Id + "\",";
                result = result + "\"text\": \"" + this.Text + "\",";
                result = result + "\"state\": { \"opened\": true, \"selected\": false },";
                result = result + "\"children\": " + this.arrayToJson(this.Childs);
                result = result + "}";
                return result;
            }

            public String arrayToJson(ICollection<CategoryJson> Categories)
            {
                String result = "[";
                if(Categories.Count > 0) {
                    foreach(CategoryJson category in Categories)
                    {
                        result = result + category.toJson() + ",";
                    }
                    result = result.Substring(0, result.Length - 1);
                }
                return result + "]";
            }
        }
    }

}
