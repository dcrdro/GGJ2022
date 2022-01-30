using UnityEngine;

// Должен висеть на том же объекте, что и аниматор
public class AnimationEventHolder : MonoBehaviour
{
    public DistanceEnemy DistanceEnemy;
    public FastEnemy FastEnemy;
    public Player Player;
    
    public void OnShotProjectile()
    {
        if (DistanceEnemy)
        {
            DistanceEnemy.ShotProjectile();
        }
    } 
    
    public void OnAttackEnded()
    {
        if (FastEnemy)
        {
            FastEnemy.EndAttack();
        }
    }  
    
    public void OnPlayerAttackEnded()
    {
        if (Player)
        {
            Player.EndMeleeAttack();
        }
    } 
    
    public void OnPlayerRangeAttackEnded()
    {
        if (Player)
        {
            Player.EndRangeAttack();
        }
    } 
}