using _Dapper.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Runtime.CompilerServices;

const string connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=Luanna21**;trustservercertificate=true;";

var category = new Category
{
    Title = "Categoria Scalar",
    Description = "Categoria Scalar",
    Url = "categoria-nova",
    Order = 13,
    Summary = "nova categoria",
    Featured = false
};

var categories = new List<Category> { category, 
    new Category{
        Id = Guid.NewGuid(),
        Title = "Categoria 2",
        Description = "Categoria 2",
        Url = "categoria-nova",
        Order = 10,
        Summary = "nova categoria",
        Featured = false  
    }, 
    new Category 
    {
        Id = Guid.NewGuid(),
        Title = "Categoria 3",
        Description = "Categoria 3",
        Url = "categoria-nova",
        Order = 11,
        Summary = "nova categoria",
        Featured = false
    }};

using (var connection = new SqlConnection(connectionString))
{
    //DeleteCategoty(connection);
    //CreateManyCategories(connection, categories);
    //ExecuteReadProcedure(connection);
    //ListCategories(connection);
    //CreateCategory(connection, category);
    //UpdateCategory(connection);
    //GetCategory(connection);
    //ExecuteScalar(connection, category);
    //ReadView(connection);
    //OneToOne(connection);
    OneToMany(connection);
}

static void ListCategories (SqlConnection connection)
{
    var categories = connection.Query<Category>("Select Id, Title From Category");
    foreach(var item in categories)
        Console.WriteLine($"{item.Id} - {item.Title}");
}
static void CreateCategory (SqlConnection connection, Category objCategory)
{
var insertSql = @"INSERT INTO 
                    Category 
                VALUES(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";
var rows = connection.Execute(insertSql, new {
        objCategory.Id,
        objCategory.Title,
        objCategory.Url,
        objCategory.Summary,
        objCategory.Order,
        objCategory.Description,
        objCategory.Featured
    });
    Console.WriteLine($"{rows} linhas inseridas");
}
static void UpdateCategory (SqlConnection connection)
{
    var updateQuery = "UPDATE Category SET Title=@title WHERE Id=@id";
    var rows = connection.Execute(updateQuery, new {
        id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
        title = "Front-End"
    });
    Console.WriteLine($"{rows} registros atualizados");
}
static void DeleteCategoty (SqlConnection connection)
{
    var deleteQuery = "DELETE FROM Category WHERE Id = @Id";
    var rows = connection.Execute(deleteQuery, new {
        id = new Guid("a09fad8f-c137-4e63-bbbd-a2d33d88b92a")
    });
    Console.WriteLine($"{rows} registros deletados");
}
static void GetCategory (SqlConnection connection)
{
    var selectQuery = "SELECT Id, Title FROM Category WHERE Id=@Id";
    var category = connection.QueryFirst<Category>(selectQuery, new {
        id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4")
    });

    Console.WriteLine($"{category.Title}");
}
static void CreateManyCategories(SqlConnection connection, List<Category> categories)
{
    var insertSql = @"INSERT INTO 
                    Category 
                VALUES(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";
    var rows = connection.Execute(insertSql, categories);
    Console.WriteLine($"{rows} linhas inseridas");
}
static void ExecuteDeleteProcedure(SqlConnection connection)
{
    var procedure = "spDeleteStudent";
    var rowsAffected = connection.Execute(procedure, new {
        StudentId = "9ec733e4-1dfc-4232-b46d-95d89149809a"
    }, commandType:System.Data.CommandType.StoredProcedure);
    Console.WriteLine($"{rowsAffected} registros afetados");
}
static void ExecuteReadProcedure(SqlConnection connection)
{
    var procedure = "spGetCoursesByCategory";
    var courses = connection.Query(procedure, new {
        CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142"
    }, commandType:System.Data.CommandType.StoredProcedure);
    
    
    foreach (var course in courses)
        Console.WriteLine($"{course.Id} - {course.Title}");
}
static void ExecuteScalar(SqlConnection connection, Category objCategory)
{
    var insertSql = @"INSERT INTO 
                    Category 
                    OUTPUT INSERTED.Id
                VALUES(NEWID(), @Title, @Url, @Summary, @Order, @Description, @Featured)";

    var id = connection.ExecuteScalar<Guid>(insertSql, new {
        objCategory.Title,
        objCategory.Url,
        objCategory.Summary,
        objCategory.Order,
        objCategory.Description,
        objCategory.Featured
    });
    Console.WriteLine($"{id} da nova categoria");
}
static void ReadView(SqlConnection connection)
{
    var sql = @"SELECT * FROM vwCourses";
    var courses = connection.Query(sql);
    foreach (var course in courses)
        Console.WriteLine($"{course.Id} - {course.Title}");
}
static void OneToOne(SqlConnection connection)
{
    var sql = @"SELECT * FROM CareerItem
                inner join Course on
                     CareerItem.CourseId = Course.Id";
    var itens = connection.Query<CareerItem, Course, CareerItem>(sql, 
                (careerItem, course) => {
                    careerItem.Course = course;
                    return careerItem;
                }, splitOn: "Id");
    foreach(var item in itens)
        Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
}
static void OneToMany(SqlConnection connection)
{
    var sql = @"SELECT
                    Career.Id,
                    Career.Title,
                    CareerItem.CareerId,
                    CareerItem.Title
                FROM 
                    Career
                INNER JOIN
                    CareerItem on CareerItem.CareerId = Career.Id
                ORDER BY
                    Career.Title";
    var careers = new List <Career>();
    var items = connection.Query<Career, CareerItem, Career>(sql,(career, item) => 
    {
        var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
        if(car == null)
        {
            car = career;
            car.Items.Add(item);
            careers.Add(car);
        } else
        {
            car.Items.Add(item);
        }
        return career;
    }, splitOn: "CareerId");

    foreach(var career in careers) {
        Console.WriteLine($"{career.Title}");
        foreach(var item in career.Items)
            Console.WriteLine($"{item.Title}");
    }
}