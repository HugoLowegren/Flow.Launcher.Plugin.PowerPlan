using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.PowerPlan
{
    public class Main : IPlugin
    {
        private static string _currentPath;
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _currentPath = context.CurrentPluginMetadata.PluginDirectory;
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            var powerList = PowerManager.GetAll();
            var results = powerList.Select(p => new Result {
                Title = p.Name,
                SubTitle = p.Name,
                IcoPath = System.IO.Path.Combine(_currentPath, p.IsActive ? @"ico\Battery-2.png" : @"ico\Battery-1.png"),
                Action = c => {
                    PowerManager.Active(p.Id);
                    _context.API.ChangeQuery($"{query.ActionKeyword} ", true);
                    return true;
                }
            });

            var s = query.FirstSearch;
            if (!string.IsNullOrEmpty(s)) {
                results = results.Where(r => r.Title.ToLower().Contains(s));
            }
            return results.ToList();
        }
    }
}
