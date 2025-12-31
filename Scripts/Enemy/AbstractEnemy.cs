using Godot;
using System;

[GlobalClass]
public abstract partial class AbstractEnemy : CharacterBody3D
{
    [Export] protected float health;
    [Signal] public delegate void take_damageEventHandler(float damage);
    public delegate void Attack();
    public Attack[] attacks;
    //Item[] Inventory;
    public virtual void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            QueueFree();
            GD.PrintRich("[color=green]Enemy defeated![/color]");
            return;
        }
        GD.PrintRich("Enemy took [color=red]" + damage + "[/color] damage!");
    }
    public override void _Ready()
    {
        Connect("take_damage", new Callable(this, "Damage"));
    }
}
