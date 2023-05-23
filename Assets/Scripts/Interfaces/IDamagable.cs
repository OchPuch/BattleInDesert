using ScriptableObjects;

namespace Interfaces
{
    public interface IDamagable
    {
        
        public void TakeDamage(int damage, UnitScript damager);
    }
}