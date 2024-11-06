namespace New_Neo_LT.Scripts
{
    public enum EInteractionType
    {
        None,
        Pickup,
        Open,
        Talk,
        Use,
        Attack,
        Interact,
        InputButtonCount
    }
    
    public interface IInteractable
    {
        public void Interact(Character character, EInteractionType interactionType);
    }
}
