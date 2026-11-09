using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

var url = Environment.GetEnvironmentVariable("FORM_URL") ?? "http://app.cloudqa.io/home/AutomationPracticeForm";
var firstName = Environment.GetEnvironmentVariable("FIRST_NAME") ?? "Jayanth";
var gender = Environment.GetEnvironmentVariable("GENDER") ?? "Male";
var state = Environment.GetEnvironmentVariable("STATE") ?? "California";
using var driver = new ChromeDriver(); // Selenium Manager resolves a compatible driver.
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
driver.Navigate().GoToUrl(url);

var nameInput = FindByLabel(driver, "First Name", "textbox", "input");
nameInput.Clear(); nameInput.SendKeys(firstName);

var genderControl = FindByLabel(driver, gender, "radio", "input");
genderControl.Click();

var stateControl = FindByLabel(driver, "State", "combobox", "select");
try { new SelectElement(stateControl).SelectByText(state, true); }
catch (NoSuchElementException) { new SelectElement(stateControl).SelectByValue(state); }

Console.WriteLine($"PASS | First Name='{nameInput.GetAttribute("value")}' | Gender='{gender}' selected | State='{state}' selected");

static IWebElement FindByLabel(IWebDriver driver, string label, string role, string tag)
{
    // Candidate discovery is semantic and scored. No id/name/class or fixed page position is assumed.
    var js = (IJavaScriptExecutor)driver;
    var candidate = js.ExecuteScript(@"
      const norm=s=>(s||'').replace(/\s+/g,' ').trim().toLowerCase();
      const target=norm(arguments[0]), role=arguments[1], tag=arguments[2];
      const controls=[...document.querySelectorAll('input,select,textarea,button,[role]')];
      const labels=[...document.querySelectorAll('label,legend,[aria-label]')];
      const score=e=>{
        if(!e.getClientRects().length || e.disabled) return -1;
        let s=0, type=(e.getAttribute('type')||'').toLowerCase();
        if(e.tagName.toLowerCase()===tag) s+=2;
        if(role==='radio' && type==='radio') s+=4;
        if(role==='textbox' && (type==='text'||e.tagName==='TEXTAREA')) s+=3;
        if(role==='combobox' && (e.tagName==='SELECT'||e.getAttribute('role')==='combobox')) s+=4;
        if(norm(e.getAttribute('aria-label'))===target) s+=8;
        if(norm(e.getAttribute('placeholder'))===target) s+=5;
        for(const l of labels){
          const lt=norm(l.innerText||l.textContent||l.getAttribute('aria-label'));
          if(lt!==target) continue;
          if(l.htmlFor && document.getElementById(l.htmlFor)===e) s+=12;
          if(l.contains(e)) s+=12;
          const near=l.closest('fieldset,[role=group],.form-group,.field,div');
          if(near && near.contains(e)) s+=6;
        }
        const parent=e.parentElement;
        if(parent && norm(parent.innerText).split(/[\n:]/)[0]===target) s+=3;
        return s;
      };
      const ranked=controls.map(e=>[e,score(e)]).filter(x=>x[1]>=6).sort((a,b)=>b[1]-a[1]);
      if(!ranked.length) return null;
      const winners=ranked.filter(x=>x[1]===ranked[0][1]);
      if(winners.length!==1) throw new Error('Ambiguous semantic candidates: '+winners.length);
      return winners[0][0];", label, role, tag);

    if (candidate is not IWebElement element) throw new NoSuchElementException($"Could not identify '{label}' from visible labels/ARIA context.");
    return element;
}
