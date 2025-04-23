namespace Game
{
  public interface EntityActions
  {
    public bool Atack(Entity other);
    public void Defend();
    public bool LooseHP(int amount);
    public bool RandAction(Entity other);

  }

  public abstract class Entity : EntityActions
  {
    protected int MaxHp;
    public void Defend()
    {
      Console.WriteLine("{0} se defendió, ganando {1} puntos de defensa", this.PersonalClass, MaxHp*5 / 100);
      Shield += MaxHp * 5 / 100;
    }
    public bool Atack(Entity other)
    {
      Console.WriteLine("{0} Ataco a {1}, inflingiendo {2} puntos de daño", this.PersonalClass, other.PersonalClass, this.Atk);
      return other.LooseHP(this.Atk);
    }
    public bool LooseHP(int amount)
    {
      if(Shield>0)
      {
        int mn = Math.Min(Shield, amount);
        Shield -= mn;
        amount -= mn;
      }
      Hp -= amount;
      return Hp <= 0;
    }

    public bool RandAction(Entity other)
    {
      Random rnd = new Random();
      int rndNumber = rnd.Next(0, 2);
      if(rndNumber==0)
      {
        Defend();
        return false;
      }
      return Atack(other);
    }
 
    public int Hp { get; protected set; } 
    public int Atk { get; protected set; }
    public int Shield { get; protected set; }
    public string PersonalClass { get; protected set; }
    public bool CPU { get; protected set; }

    public override string ToString() => PersonalClass + "\nHp :"+Hp+"\nShield: "+Shield+"\n";
  }

  class Hero : Entity
  {

    Dictionary<string, int[]> stats = new Dictionary<string, int[]>();
    public Hero(string clase, bool isControledByPlayer)
    {
      StartStats();
      MaxHp = stats[clase][0];
      Atk = stats[clase][1];
      Shield = stats[clase][2];
      Hp = MaxHp;
      PersonalClass = clase.ToString();
      CPU = !isControledByPlayer;
    }
    private void StartStats()
    {
      stats["Warrior"] = [100, 15, 40];
      stats["Archer"] = [100, 30, 20];
      stats["Mage"] = [120, 10, 10];
    }
  }

  class Enemy : Entity
  {
    Dictionary<string, int[]> stats = new Dictionary<string, int[]>();

    public Enemy(string clase, bool isControledByPlayer)
    {
      StartStats();

      MaxHp = stats[clase][0];
      Atk = stats[clase][1];
      Shield = stats[clase][2];
      Hp = MaxHp;
      PersonalClass = clase.ToString();
      CPU = !isControledByPlayer;
    }

    private void StartStats()
    {
      stats["skeleton"] = [50, 20, 30];
      stats["zombie"] = [100, 10, 10];
      stats["litch"] = [200, 30, 10];
    }
  }


  public interface Schema
  {
    public void Start(Entity P1, Entity P2);
    public void End(Entity P1);
    public void Status(Entity P1, Entity P2);
  }

  class Battle : Schema
  {
    public void Start(Entity P1, Entity P2) 
    {
      Console.WriteLine("Ha empezado la batalla entre {0} y {1}, que gane el mejor!", P1.PersonalClass, P2.PersonalClass);

      bool MakeSomeAction(Entity a, Entity b)
      {
        if (a.CPU) return a.RandAction(b);
        int option=0;
        do
        {
          Console.WriteLine("Que hara {0}?\nAtacar(1), Defender(2)", a.PersonalClass);
          if (int.TryParse(Console.ReadLine(), out option)) continue;
        } while (option < 1 || option > 2) ;
        if(option==1) return a.Atack(b);
        a.Defend();
        return false;
      }
      while (true)
      {
        Status(P1, P2);
        if(MakeSomeAction(P1, P2))
        {
          End(P1);
          break;
        }
        if(MakeSomeAction(P2, P1))
        {
          End(P2);
          break;
        }
      }
    }
    public void End(Entity winner) => Console.WriteLine("Ha ganado {0}, felicidades!", winner.PersonalClass);
    public void Status(Entity P1, Entity P2)
    {
      Console.WriteLine("{0}\n{1}", P1, P2);
    }
  }


  class MainMenu
  {
    public static void Main(string[] args)
    {
      Hero player;
      Enemy enemigo;
      Console.WriteLine("Bienvenido al juego");
      do
      {
        Console.WriteLine("Selecciona un jugador\nClases disponibles: Warrior, Archer, Mage");
        string s = Console.ReadLine();
        if (s != "Warrior" && s != "Archer" && s != "Mage") continue;
        player = new Hero(s, true);
        break;
      } while (true);
      do
      {
        Console.WriteLine("Seleccione un enemigo\nClases disponibles: zombie, skeleton, litch");
        string s = Console.ReadLine();
        if (s != "zombie" && s != "skeleton" && s != "litch") continue;
        enemigo = new Enemy(s, false);
        break;
      } while (true);

      Schema schema = new Battle();

      schema.Start(player, enemigo);

     
    }
  }
}