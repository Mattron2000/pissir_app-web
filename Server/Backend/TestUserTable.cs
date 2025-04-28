namespace Backend;

using Backend.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class TestUserTable
{
    public void Test()
    {
        var context = new SmartParkingContext();

        using var connection = new SqliteConnection(context.Database.GetConnectionString());
        connection.Open();

        // Mostra tipi disponibili
        Console.WriteLine("✅ Tipi disponibili:");
        foreach (var type in context.UserTypes)
        {
            Console.WriteLine($"- {type.Name}");
        }

        // Mostra lo stato della tabella Users
        Console.WriteLine("✅ Stato della tabella Users:");
        var users = context.Users.ToList();
        if (users.Count == 0)
        {
            Console.WriteLine("❌ Tabella Users vuota");
        }
        else
        {
            Console.WriteLine("✅ Tabella Users non vuota");
            foreach (var user in users)
            {
                Console.WriteLine($"- Email: {user.Email}, Password: {user.Password}, Nome: {user.Name}, Cognome: {user.Surname}, Tipo: {user.Type}");
            }
        }

        // // ✅ Inserimenti multipli validi (BASE)
        // for (int i = 0; i < 5; i++)
        // {
        //     var user = new User
        //     {
        //         Email = $"user{i}@example.com",
        //         Password = "password",
        //         Name = "John",
        //         Surname = "Doe",
        //         Type = UserTypeEnum.BASE
        //     };
        //     context.Users.Add(user);
        //     context.SaveChanges();
        //     Console.WriteLine("✅ User salvato con successo.");
        // }

        // // ✅ Inserimenti multipli validi
        // for (int i = 0; i < 5; i++)
        // {
        //     context.Slots.Add(new Slot
        //     {
        //         Status = SlotStatusEnum.OCCUPIED
        //     });
        //     context.SaveChanges();
        //     Console.WriteLine("✅ Slot salvato con successo.");
        // }

        // // Controlla nuovamente lo stato della tabella Slots
        // Console.WriteLine("✅ Stato aggiornato della tabella Slots:");
        // var updatedSlots = context.Slots.ToList();
        // if (updatedSlots.Count == 0)
        // {
        //     Console.WriteLine("❌ Tabella Slots vuota");
        // }
        // else
        // {
        //     Console.WriteLine("✅ Tabella Slots non vuota");
        //     foreach (var slot in updatedSlots)
        //     {
        //         Console.WriteLine($"- ID: {slot.Id}, Stato: {slot.Status}");
        //     }
        // }

        // // ❌ Inserimento non valido (tipo inesistente)
        // try
        // {
        //     context.Slots.Add(new Slot
        //     {
        //         Status = (SlotStatusEnum)999 // non esiste in tabella PriceType
        //     });
        //     context.SaveChanges();
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine("❌ Errore inserendo tipo non valido:");
        //     Console.WriteLine(ex.Message);
        //     Console.WriteLine(ex.InnerException?.Message);
        // }

        // // Resetta il database
        // if (context.Slots.Any())
        // {
        //     Console.WriteLine("⚠️ Tabella Slots popolata. Procedo con il reset...");
        //     context.Slots.RemoveRange(context.Slots);
        //     context.SaveChanges();
        //     Console.WriteLine("✅ Tabella Slots resettata.");
        // }
        // else
        // {
        //     Console.WriteLine("✅ Tabella Slots già vuota. Nessuna azione necessaria.");
        // }
    }
}
