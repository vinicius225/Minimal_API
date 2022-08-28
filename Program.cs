using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContex>(options => options.UseInMemoryDatabase("TarefaDb"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", ()=> "Olá Mundo");


app.MapGet("/tarefa", async (AppDbContex db) =>
{
    return await db.Tarefa.ToListAsync();
});

app.MapPost("/tarefa", async (Tarefa tarefa,AppDbContex db) =>
{
    db.Add(tarefa);
   await db.SaveChangesAsync();
    return Results.Created("Criado com sucesso!", tarefa);
});

app.MapGet("/tarefas/{id}",  (int id, AppDbContex db) =>
{
     var tarefa =    db.Tarefa.Where(t => t.Id == id).FirstOrDefault();
    if(tarefa == null)
    {
        return Results.NotFound();
    }
  return Results.Ok(tarefa);
});

app.MapPut("/tarefa/{id}", (int id, Tarefa obj, AppDbContex db)=>
{
    var resource = db.Tarefa.Where(a => a.Id == id).FirstOrDefault();

    if(resource == null)
    {
        return Results.NotFound();
    }
    else
    {
        resource = obj;
        db.Update(resource);
        db.SaveChanges();

        return Results.Ok(resource);
    }

});

app.MapGet("frases", async () => 
await new HttpClient().GetStringAsync("https://ron-swanson-quotes.herokuapp.com/v2/quotes")
);


app.Run();


class Tarefa
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsConcluida { get; set; }

}


class AppDbContex : DbContext
{
    public AppDbContex(DbContextOptions<AppDbContex> options ): base(options)
    {

    }
    public DbSet<Tarefa> Tarefa { get; set; }
}

