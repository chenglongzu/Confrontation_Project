namespace ShimmerFramework
{
    public class MainUiCanvas : BasePanel
    {
        public override void Start()
        {
            base.Start();
            gameObject.AddComponent<UIAdaptation>();
            DontDestroyOnLoad(gameObject);
        }
    }

}
