using Godot;

namespace Fractural.Utils
{
    public static class ProjectSettingsUtils
    {
        /// <summary>
        /// Returns the value of setting. 
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetSetting<T>(string name)
        {
            return (T)ProjectSettings.GetSetting(name);
        }
    }
}