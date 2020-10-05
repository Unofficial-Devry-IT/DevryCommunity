public interface IDamagable
{
    float Health { get; }
    void TakeDamage(float amount);
    void Heal(float amount);
}

public class Actor : IDamagable
{
    float _health = 0;
    float _maxHealth = 0;
    public float Health => _health;

    public Actor(int startingHealth)
    {
        _health = _maxHealth = startingHealth;
    }

    private void Die()
    {
        Console.WriteLine("I died");
    }

    public void Reset()
    {
        _health = _maxHealth;
    }

    public void TakeDamage(float amount)
    {
        _health -= amount;
        Console.WriteLine(Health);
        if (_health <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (_health + amount > _maxHealth)
            _health = _maxHealth;
        else
            _health += amount;

        Console.WriteLine(Health);
    }
}

Actor one = new Actor(100);
one.TakeDamage(10f);
one.Heal(4);

IDamagable damagable = one;
damagable.TakeDamage(3f);
damagable.Heal(20f);