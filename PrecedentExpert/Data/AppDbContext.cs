namespace PrecedentExpert.Data;

using Microsoft.EntityFrameworkCore;
using PrecedentExpert.Models;
using System;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        try
        {
            // Пытаемся установить соединение с базой данных
            Database.OpenConnection();
            Database.CloseConnection();
        }
        catch (Exception ex)
        {
            // Обработка ошибки подключения
            Console.WriteLine($"Error connecting to the database: {ex.Message}");
            throw;
        }
    }

    public DbSet<SituationVariable> SituationVariables { get; set; }
    public DbSet<SolutionVariable> SolutionVariables { get; set; }
    public DbSet<Precedent> Precedents { get; set; }
    public DbSet<Objects> Objects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
