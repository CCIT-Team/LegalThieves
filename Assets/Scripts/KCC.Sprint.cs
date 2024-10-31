namespace Fusion.Addons.KCC
{
    /// <summary>
    /// Partial implementation of KCC class to extend public API with sprint functionality.
    /// </summary>
    public partial class KCC
    {
        // PUBLIC METHODS

        public void SetSprint(bool sprint)
        {
            // Solution No. 1
            // Set Sprint property in KCCData instance. This assignment is done ONLY on current KCCData instance (_fixedData for fixed update, _renderData for render update).
            // If you call this method after KCC fixed update, it will NOT propagate to render for the same frame.
            // Data.Sprint = sprint;

            // Solution No. 2
            // More correct approach in this case is to explicitly set Sprint for render data and fixed data.
            // This way you'll not lose sprint information for following render frames if the method is called after fixed update.
            _renderData.Sprint = sprint;

            if (IsInFixedUpdate == true)
            {
                _fixedData.Sprint = sprint;
            }

            // To prevent visual glitches, it is highly recommended to call SetSprint() always before the KCC update.
            // Ideally put some asserts to make sure execution order is correct.
        }
    }
}

