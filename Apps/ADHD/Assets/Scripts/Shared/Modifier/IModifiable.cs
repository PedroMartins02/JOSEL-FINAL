namespace Game.Logic.Modifiers
{
    public interface IModifiable 
    {
        public void ApplyModifier(Modifier modifier);
        public void RemoveModifier(Modifier modifier);
    }
}
