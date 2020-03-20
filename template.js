var currentPage = 1;
var maxPage = 1;

function clickOrigin(e){
    var target = e.target;
    var tag = [];
    tag.tagType = target.tagName.toLowerCase();
    tag.tagClass = target.className.split(' ');
    tag.id = target.id;
    tag.parent = target.parentNode;

    return tag;
}

function nextPage()
{
  if(currentPage == maxPage)
  {
    return;
  }
  if(currentPage < maxPage)
  {
    var cPage =".page"+currentPage;
    currentPage+=1;
    var nPage =".page"+currentPage;
    $(cPage).hide();
    $(nPage).show();
  }
}

function previousPage()
{
  if(currentPage == 1)
  {
    return;
  }
  if(currentPage >1)
  {
    var cPage =".page"+currentPage;
    currentPage-=1;
    var pPage =".page"+currentPage;
    $(cPage).hide();
    $(pPage).show();
  }
 
}

$(document).ready(function() 
{
   var numS = 1;
  while (numS <= maxPage)
    {
      if(numS == 1)
        {
          numS++;
        }
      else
        {
          if(numS <= maxPage)
          {
            var tempPage =".page"+numS;
            $(tempPage).hide();
            numS++;
          }
        }
    }
});

document.body.onclick = function(e)
{
    elem = clickOrigin(e);
    if(elem.tagClass == 'previous')
      {
        previousPage();
      }
    if(elem.tagClass == 'next')
      {
        nextPage();
      }
    if(elem.tagClass == 'numbered')
      {
        if(elem.id != 'numberPages')
          {
            var ID = elem.id;
            var numTo = ID.substring(1);
            if(numTo != currentPage)
              {
                if(numTo > currentPage)
                  {
                    while(numTo > currentPage)
                      {
                        nextPage();
                      }
                  }
                if(numTo < currentPage)
                  {
                    while(numTo < currentPage)
                      {
                        previousPage();
                      }
                  }
              }
            
          }
      }
};