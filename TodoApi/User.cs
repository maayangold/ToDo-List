using System;
using System.Collections.Generic;
//התממשקות למסד הנתונים ויצירת טבלאות על פיו
//dotnet ef dbcontext scaffold Name=ToDoDB Pomelo.EntityFrameworkCore.MySql  -f -c ToDoDbContext

namespace TodoApi;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}
