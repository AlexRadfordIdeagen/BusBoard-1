using System.Collections.Generic;

namespace BusBoard.Web.ViewModels
{
  public class BusInfo
  {
    public BusInfo(List<string> info)
    {
      Info = info;
    }

    public List<string> Info { get; set; }

  }
}