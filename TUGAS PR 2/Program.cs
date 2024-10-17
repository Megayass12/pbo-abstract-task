using System;
using System.Collections.Generic;

interface IKemampuan
{SS
    void Gunakan(Robot pengguna, Robot target); // Menggunakan kemampuan pada target
    bool IsCooldown();         // Memeriksa apakah kemampuan dalam cooldown
    void ResetCooldown();      // Mengatur ulang cooldown setelah durasi selesai
    void UpdateCooldown();     // Mengurangi durasi cooldown setiap giliran
}

abstract class Robot
{
    public string Nama;
    protected int Energi;
    protected int Armor;
    protected int Serangan;

    // Pembuatan Default Stat untuk constructor robot
    private static int defaultEnergi = 100;
    private static int defaultArmor = 10;
    private static int defaultSerangan = 20;

    public Robot(string nama, int? energi = null, int? armor = null, int? serangan = null)
    {
        Nama = nama;
        Energi = energi ?? defaultEnergi; // Menggunakan Default Stat apabila tidak dibuat
        Armor = armor ?? defaultArmor; // Menggunakan Default Stat apabila tidak dibuat
        Serangan = serangan ?? defaultSerangan; // Menggunakan Default Stat apabila tidak dibuat
    }
    //Karena properti Energi, Armor, dan Serangan protected memakai function get set
    //dibawah ini untuk memperbarui nilai properti tersebut
    public int GetEnergi() { return Energi; }
    public int GetArmor() { return Armor; }
    public int GetSerangan() { return Serangan; }

    public void SetEnergi(int nilai) { Energi = nilai; }
    public void SetArmor(int nilai) { Armor = nilai; }
    public void SetSerangan(int nilai) { Serangan = nilai; }

    public abstract void Serang(Robot target);

    public void GunakanKemampuan(IKemampuan kemampuan, Robot target)
    {
        if (!kemampuan.IsCooldown())
        {
            kemampuan.Gunakan(this, target);
        }
        else
        {
            Console.WriteLine("Kemampuan sedang dalam cooldown.");
        }
    }

    public void CetakInformasi()
    {
        Console.WriteLine($"Nama: {Nama}, Energi: {Energi}, Armor: {Armor}, Serangan: {Serangan}");
    }
}

class BosRobot : Robot //Properti Robot Bos
{
    private bool isStunned = false; // Properti untuk mengecek apakah bos sedang stun

    public BosRobot(string nama, int energi, int armor, int serangan)
        : base(nama, 100 + energi, 2 + armor, 5 + serangan) { }

    //Karena properti stun private memakai function get set
    public bool getStunned() { return isStunned; }
    public void SetStunned(bool value) { isStunned = value; }

    public override void Serang(Robot target)
    {
        if (isStunned)
        {
            Console.WriteLine($"{Nama} sedang terkena stun dan tidak dapat menyerang!");
            isStunned = false; // Stun hanya berlaku 1 turn, jadi reset setelah giliran ini
            return;
        }

        int damage = Math.Max(0, Serangan - target.GetArmor()); //Properti mengecek berapa banyak serangan yang tembus
        target.SetEnergi(target.GetEnergi() - damage); //Properti mengurangi energi lawan dengan serangan yang tembus
        Console.WriteLine($"{Nama} menyerang {target.Nama} mengurangi energinya sebesar {damage}");

        if (target.GetEnergi() <= 0)
        {
            Console.WriteLine($"{target.Nama} telah kalah!");
        }
    }
}

class RobotTempur : Robot
{
    public RobotTempur(string nama)
        : base(nama) // Menggunakan stat Default
    {
    }

    public override void Serang(Robot target)
    {
        int damage = Math.Max(0, GetSerangan() - target.GetArmor()); //Properti mengecek berapa banyak serangan yang tembus
        target.SetEnergi(target.GetEnergi() - damage); //Properti mengurangi energi lawan dengan serangan yang tembus
        Console.WriteLine($"{Nama} menyerang {target.Nama} dan mengurangi energinya sebesar {damage}");

        if (target.GetEnergi() <= 0)
        {
            Console.WriteLine($"{target.Nama} telah kalah!");
        }
    }
}

class Perbaikan : IKemampuan
{
    private bool cooldown = false; // Variabel Cooldown aktif atau tidak
    private int cooldownCounter = 0; // Counter mengecek Cooldown berapa giliran
    private const int cooldownDuration = 2; // Cooldown berlangsung 2 giliran

    public void Gunakan(Robot pengguna, Robot target)
    {
        pengguna.SetEnergi(pengguna.GetEnergi() + 30);
        Console.WriteLine($"{pengguna.Nama} menggunakan Perbaikan, energi bertambah 20 poin.");
        cooldown = true;
        cooldownCounter = cooldownDuration;
    }

    public bool IsCooldown() { return cooldown; }

    public void ResetCooldown()
    {
        cooldown = false;
        cooldownCounter = 0;
    }

    public void UpdateCooldown()
    {
        if (cooldown && cooldownCounter > 0)
        {
            cooldownCounter--;
            if (cooldownCounter == 0)
                ResetCooldown();
        }
    }
}

class SeranganListrik : IKemampuan
{
    private bool cooldown = false; // Variabel Cooldown aktif atau tidak
    private int cooldownCounter = 0; // Counter mengecek Cooldown berapa giliran
    private const int cooldownDuration = 2; // Cooldown berlangsung 2 giliran

    public void Gunakan(Robot pengguna, Robot target)
    {
        int baseDamage = 20;
        int actualDamage = Math.Max(0, baseDamage - target.GetArmor());
        target.SetEnergi(target.GetEnergi() - actualDamage);
        Console.WriteLine($"{pengguna.Nama} menggunakan Serangan Listrik pada {target.Nama}, mengurangi energi {target.Nama} sebesar {actualDamage} poin.");

        if (target is BosRobot bos)
        {
            bos.SetStunned(true); // Memberikan efek stun pada bos
            Console.WriteLine($"{target.Nama} terkena stun dan tidak dapat menyerang untuk 1 giliran!");
        }

        cooldown = true;
        cooldownCounter = cooldownDuration;
    }

    public bool IsCooldown() { return cooldown; }

    public void ResetCooldown()
    {
        cooldown = false;
        cooldownCounter = 0;
    }

    public void UpdateCooldown()
    {
        if (cooldown && cooldownCounter > 0)
        {
            cooldownCounter--;
            if (cooldownCounter == 0)
                ResetCooldown();
        }
    }
}

class SeranganPlasma : IKemampuan
{
    private bool cooldown = false; // Variabel Cooldown aktif atau tidak
    private int cooldownCounter = 0; // Counter mengecek Cooldown berapa giliran
    private const int cooldownDuration = 3; // Cooldown berlangsung 3 giliran

    public void Gunakan(Robot pengguna, Robot target)
    {
        int damage = 25;
        target.SetEnergi(target.GetEnergi() - damage);
        Console.WriteLine($"{pengguna.Nama} menggunakan Serangan Plasma, menembus armor dan mengurangi energi {target.Nama} sebesar {damage} poin.");
        cooldown = true;
        cooldownCounter = cooldownDuration;
    }

    public bool IsCooldown() { return cooldown; }

    public void ResetCooldown()
    {
        cooldown = false;
        cooldownCounter = 0;
    }

    public void UpdateCooldown()
    {
        if (cooldown && cooldownCounter > 0)
        {
            cooldownCounter--;
            if (cooldownCounter == 0)
                ResetCooldown();
        }
    }
}

class PertahananSuper : IKemampuan
{
    private bool cooldown = false; // Variabel Cooldown aktif atau tidak
    private int cooldownCounter = 0; // Counter mengecek Cooldown berapa giliran
    private const int cooldownDuration = 3; // Cooldown berlangsung 3 giliran
    private int temporaryArmorIncrease = 10; //Properti total armor yang meningkat
    private int temporaryArmorDuration = 2; // Armor tambahan berlangsung 2 giliran
    private int remainingArmorDuration = 0; // Counter mengecek armor berapa giliran
    private Robot pengguna; // Menyimpan referensi ke pengguna

    public void Gunakan(Robot pengguna, Robot target)
    {
        this.pengguna = pengguna; // Simpan referensi pengguna
        pengguna.SetArmor(pengguna.GetArmor() + temporaryArmorIncrease);
        remainingArmorDuration = temporaryArmorDuration;
        Console.WriteLine($"{pengguna.Nama} menggunakan Pertahanan Super, meningkatkan armor sebanyak {temporaryArmorIncrease} poin untuk {temporaryArmorDuration} giliran.");
        cooldown = true;
        cooldownCounter = cooldownDuration;
    }

    public bool IsCooldown() { return cooldown; }

    public void ResetCooldown()
    {
        cooldown = false;
        cooldownCounter = 0;
    }

    public void UpdateCooldown()
    {
        if (cooldown && cooldownCounter > 0)
        {
            cooldownCounter--;
            if (cooldownCounter == 0)
                ResetCooldown();
        }

        // Update durasi armor sementara
        if (remainingArmorDuration > 0)
        {
            remainingArmorDuration--;
            if (remainingArmorDuration == 0 && pengguna != null)
            {
                // Mengembalikan armor ke nilai asli setelah efek berakhir
                Console.WriteLine("Efek Pertahanan Super telah berakhir, armor kembali normal.");
                pengguna.SetArmor(pengguna.GetArmor() - temporaryArmorIncrease);
                pengguna = null; // Hapus referensi pengguna setelah efek selesai
            }
        }
    }
}


class SimulatorPertarungan
{
    static int turnCounter = 0; // Counter untuk giliran

    static void Main(string[] args)
    {
        Robot robot = new RobotTempur("Alpha");
        BosRobot bos = new BosRobot("Omega", 100, 10, 20);

        IKemampuan perbaikan = new Perbaikan();
        IKemampuan listrik = new SeranganListrik();
        IKemampuan plasma = new SeranganPlasma();
        IKemampuan perisai = new PertahananSuper();

        Dictionary<string, IKemampuan> abilities = new Dictionary<string, IKemampuan>()
        {
            { "2", perbaikan },
            { "3", listrik },
            { "4", plasma },
            { "5", perisai },
        };

        bool gameOver = false;
        while (!gameOver) // Looping agar aksi robot vs bos robot selalu berlangsung
        {
            turnCounter++;

            // Aksi Robot menyerang
            Console.WriteLine("\n--- Giliran Robot 1 (Alpha) ---");
            robot.CetakInformasi();
            Console.WriteLine("Pilihan: \n1. Serang \n2. Perbaikan \n3. Serangan Listrik \n4. Serangan Plasma \n5.Pertahanan Super");
            string input1 = Console.ReadLine();
            Console.Clear();

            if (input1 == "1")
                robot.Serang(bos);
            else if (abilities.ContainsKey(input1))
                robot.GunakanKemampuan(abilities[input1], bos);

            // Update cooldowns setelah aksi
            UpdateAllCooldown(abilities);

            // Aturan if Bos Robot kalah, Robot Menang
            if (bos.GetEnergi() <= 0)
            {
                gameOver = true;
                Console.WriteLine($"{bos.Nama} telah dikalahkan! {robot.Nama} menang!");
                break;
            }

            // Aksi Bos Robot menyerang
            Console.WriteLine("\n--- Giliran Bos (Omega) ---");
            bos.CetakInformasi();
            bos.Serang(robot);

            // Aturan if Bos Robot Menang, Robot kalah
            if (robot.GetEnergi() <= 0)
            {
                gameOver = true;
                Console.WriteLine($"{robot.Nama} telah dikalahkan! \n--- GAME OVER ---");
                break;
            }

            // Aturan memperbarui stat serangan dan armor robot setelah 3 giliran
            if (turnCounter % 3 == 0)
            {
                PerbaruiStat(robot);
                PerbaruiStat(bos);
            }
        }
    }

    static void UpdateAllCooldown(Dictionary<string, IKemampuan> abilities) //Function untuk memperbarui hitungan semua cooldown
    {
        foreach (var ability in abilities.Values)
        {
            ability.UpdateCooldown();
        }
    }

    static void PerbaruiStat(Robot robot) //Function aturan perubahan armor dan serangan robot
    {
        Random rand = new Random();
        int perubahanArmor = rand.Next(-3, 4); // Perubahan antara -3 hingga 3
        int perubahanSerangan = rand.Next(-3, 4); // Perubahan antara -3 hingga 3

        robot.SetArmor(robot.GetArmor() + perubahanArmor);
        robot.SetSerangan(robot.GetSerangan() + perubahanSerangan);

        Console.WriteLine($"\nPerubahan Stat untuk {robot.Nama}: Armor {perubahanArmor}, Serangan {perubahanSerangan}");
    }
}