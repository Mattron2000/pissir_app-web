namespace Backend;

using Backend.Data;
using Backend.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class TestPriceTable
{
    public void Test()
    {
        var context = new SmartParkingContext();

        using var connection = new SqliteConnection(context.Database.GetConnectionString());
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys;";
        var result = command.ExecuteScalar();
        if (result == null)
        {
            Console.WriteLine("❌ PRAGMA foreign_keys: null");
            return;
        }
        if (result.ToString() == "0")
        {
            Console.WriteLine("❌ PRAGMA foreign_keys: 0");
            return;
        }
        else if (result.ToString() == "1")
        {
            Console.WriteLine("✅ PRAGMA foreign_keys: 1");
        }
        else
        {
            Console.WriteLine($"❌ PRAGMA foreign_keys: {result}");
            return;
        }

        // Mostra tipi disponibili
        Console.WriteLine("✅ Tipi disponibili:");
        foreach (var type in context.PriceTypes)
        {
            Console.WriteLine($"- {type.Name}");
        }

        // Mostra lo stato della tabella Prices
        Console.WriteLine("✅ Stato della tabella Prices:");
        var prices = context.Prices.ToList();
        if (prices.Count == 0)
        {
            Console.WriteLine("❌ Tabella Prices vuota");
        }
        else
        {
            Console.WriteLine("✅ Tabella Prices non vuota");
            foreach (var price in prices)
            {
                Console.WriteLine($"- Tipo: {price.Type}, Importo: {price.Amount}");
            }
        }

        // // Resetta il database
        // context.Prices.RemoveRange(context.Prices);
        // context.SaveChanges();

        // // ✅ Inserimento valido
        // context.Prices.Add(new Price
        // {
        //     Type = PriceTypeEnum.PARKING,
        //     Amount = 2.50
        // });
        // context.SaveChanges();
        // Console.WriteLine("✅ Prezzo PARKING salvato con successo.");

        // // ✅ Inserimento valido
        // context.Prices.Add(new Price
        // {
        //     Type = PriceTypeEnum.CHARGING,
        //     Amount = 4.50
        // });
        // context.SaveChanges();
        // Console.WriteLine("✅ Prezzo CHARGING salvato con successo.");

        // // ✅ Re-inserimento valido
        // try
        // {
        //     var entity = context.Prices.Add(new Price
        //     {
        //         Type = PriceTypeEnum.PARKING,
        //         Amount = 3.50
        //     });
        //     context.SaveChanges();
        //     Console.WriteLine("❌ Prezzo PARKING reinserito con successo.");
        //     Console.WriteLine($"- Tipo: {entity.Entity.Type}, Importo: {entity.Entity.Amount}");
        // }
        // catch (InvalidOperationException ex)
        // {
        //     Console.WriteLine("✅ Errore reinserimento prezzo PARKING");
        //     Console.WriteLine(ex.Message);
        // }

        // // ❌ Inserimento non valido (tipo inesistente)
        // try
        // {
        //     context.Prices.Add(new Price
        //     {
        //         Type = (PriceTypeEnum)999, // non esiste in tabella PriceType
        //         Amount = 4.99
        //     });
        //     context.SaveChanges();
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine("❌ Errore inserendo tipo non valido:");
        //     Console.WriteLine(ex.Message);
        //     Console.WriteLine(ex.InnerException?.Message);
        // }

        // // Stampa tutte le righe presenti nella tabella Prices
        // Console.WriteLine("✅ Righe presenti nella tabella Prices:");
        // var prices = context.Prices
        //     .Include(p => p.PriceType) // Include la navigazione alla tabella PriceType
        //     .ToList();
        // foreach (var price in prices)
        // {
        //     Console.WriteLine($"- Tipo: {price.Type}, Importo: {price.Amount}");
        // }
    }
}
