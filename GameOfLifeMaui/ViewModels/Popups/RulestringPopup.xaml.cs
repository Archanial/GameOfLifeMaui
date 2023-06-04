using GameOfLifeMaui.ViewModels.Pages;

namespace GameOfLifeMaui.ViewModels.Popups;

public sealed partial class RulestringPopup
{
    public RulestringPopup()
    {
        InitializeComponent();

        var rulestring = SettingsManager.GetRuleString.Split("/");
        BKey.Text = rulestring[0][1..];
        SKey.Text = rulestring[1][1..];
    }

    private void CancelButtonClicked(object sender, EventArgs e) => Close();

    private async void ConfirmButtonClicked(object sender, EventArgs e)
    {
        var newS = (from character in SKey.Text where char.IsDigit(character) select character - '0')
            .ToArray();
        var newB = (from character in BKey.Text where char.IsDigit(character) select character - '0')
            .Distinct()
            .ToArray();
        Array.Sort(newB);
        Array.Sort(newS);
        await SettingsManager.SetRulestring(newB, newS);
        
        if (Shell.Current.CurrentPage is SettingsPage settingsPage)
        {
            settingsPage.UpdateRulestring();
        }
        
        Close();
    }

    private void BKeyOnCompleted(object sender, EventArgs e)
    {
        if (SKey.Text != null)
        {
            SKey.Placeholder = SKey.Text;
            SKey.Text = null;
        }
        
        SKey.Focus();
    }

    private void SKeyOnCompleted(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(BKey.Text))
        {
            if (BKey.Text != null)
            {
                BKey.Placeholder = BKey.Text;
                BKey.Text = null;
            }
            
            BKey.Focus();
            return;
        }

        ConfirmButtonClicked(sender, e);
    }
}