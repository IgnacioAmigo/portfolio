using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PortfolioWebNET.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PortfolioWebNET.Services
{
    public class BlogService
    {
        private static BlogService instance;
        private CloudStorageAccount storageAccount;
        private CloudTableClient cloudTableClient;
        private CloudTable tableReference;
        private BlogService() {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["portfoliosa_AzureStorageConnectionString"]);
            cloudTableClient = storageAccount.CreateCloudTableClient();
            tableReference = cloudTableClient.GetTableReference("BlogPosts");

            tableReference.CreateIfNotExists();
        }

        public static BlogService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BlogService();
                }
                return instance;
            }
        }

        public TableResult AddBlog(BlogPost bp)
        {
            TableOperation insertOperation = TableOperation.Insert(bp);
            TableResult result = tableReference.Execute(insertOperation);

            return result;
        }

        public TableResult DeleteBlog(BlogPost bp)
        {
            TableOperation insertOperation = TableOperation.Delete(bp);
            TableResult result = tableReference.Execute(insertOperation);

            return result;
        }

        public List<BlogPost> GetLatest5()
        {

            List<BlogPost> result = new List<BlogPost>();
            TableQuery<BlogPost> query = new TableQuery<BlogPost>().Take(5);
            var tresult = tableReference.ExecuteQuery(query);
            foreach (BlogPost m in tresult)
            {
                result.Add(m);
            }

            return result;
        }


        public TableResult UpdateBlog(BlogPost bp)
        {
            BlogPost bp_ = GetBlog(bp);
            
            if (bp_ != null)
            {

                bp_.Text = bp.Text;
                bp_.TimeStamp = bp.TimeStamp;
                bp_.Title = bp.Title;
            }


                 TableOperation insertOperation = TableOperation.Replace(bp_);

            TableResult result = tableReference.Execute(insertOperation);

            return result;
        }

        public BlogPost GetBlog(BlogPost bp)
        {
            List<BlogPost> result = new List<BlogPost>();
            TableQuery<BlogPost> query = new TableQuery<BlogPost>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,bp.PartitionKey));
            var tresult = tableReference.ExecuteQuery(query);

            return ((BlogPost)tresult.First());
        }

    }
}