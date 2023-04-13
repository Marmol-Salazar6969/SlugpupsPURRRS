namespace SlugpupsPurrs
{
    public class CryAboutIt
    {
        public static SoundID purrr;
        public static SoundID meow;

        public static void RegisterValues()
        {
            purrr = new SoundID("purrr", true);
            meow = new SoundID("meow", true);
        }
    }
}
