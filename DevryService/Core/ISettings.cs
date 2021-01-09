using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    public interface ISettings<TOptions>
        where TOptions : WizardConfig
    {
        TOptions DefaultSettings();
        CommandConfig DefaultCommandConfig();
        void LoadSettings(TOptions options);
    }
}
