using System;
using System.Data.Entity;
using System.Linq;

namespace SIAA.Models
{
    public class AsesoriaModel : DbContext
    {
        // Your context has been configured to use a 'AsesoriaModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'SIAA.Models.AsesoriaModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'AsesoriaModel' 
        // connection string in the application configuration file.
        public AsesoriaModel()
            : base("name=AsesoriaModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}