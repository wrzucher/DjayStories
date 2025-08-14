using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DjayStories.Core;

[Display (Name = "Пьеса", Description = @"
Общее описание пьесы. Ты и все остальные являются её участниками.
Необходимо вжиться в пьесу и играть назначенную роль, даже если она отрицательная.
Необходимо следовать инструкциям и достигать целей, поставленных перед ролью
")]
public class Game
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Context { get; set; } = "";

    public ICollection<Player> Players { get; set; } = new List<Player>();
}

public class Player
{
    public int GameId { get; set; }
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public bool IsReal { get; set; }

    public int RoleId { get; set; }

    public Role Role { get; set; }
    public Game Game { get; set; }
}

public class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string HowHeFeels { get; set; } = "";

    public string Target { get; set; } = "";

    public ICollection<Player> Players { get; set; } = new List<Player>();
}

public class Responce
{
    public string Target { get; set; } = "";
    public string Speach { get; set; } = "";
    public string Action { get; set; } = "";
}

public class DisplayEntry
{
    public string Name { get; set; }
    public string Description { get; set; }
    public object? Value { get; set; }
}

public class Resquest
{
    public string MainRequirements { get; set; } = "";
    public string YourName { get; set; } = "";
    public string Instructions { get; set; } = "";

    public object Context { get; set; }
}