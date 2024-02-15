

var tabSep = "titleTab";
var focusedTabIndx = 0;
function tab(title, width, div_name, func, rowsNumber)
{
	this.title 	= title;
	this.divName	= div_name;
	this.func       = func;
	this.width 	= width;
	this.rowsNumber	= rowsNumber;
	this.order      = null;
}

function collectTabs()
{
	this.tabs = new Array();
	this.titleTblWidth = 0;
	for(i=0;i<arguments.length;i++)
	{
		this.tabs[this.tabs.length] = arguments[i];
		//alert(this.tabs[this.tabs.length-1])
		this.tabs[this.tabs.length-1].order = parseInt(i)+1;
	}
	for(i=0;i<this.tabs.length;i++)
	{
		if(this.tabs[i].divName == focusedTab)
		{
			master_less_for_dt();
		}
	}

	this.drawTabs = function(divWidth)
	{
		this.titleTbl = '<ul id="navlist" class="nav nav-tabs" style="width:'+divWidth+';margin:0px;" selectedTab="'+tabSep+focusedTab+'" >';
		//focusedTab = this.tabs[0].divName;
		this.titleTbl+= "";
		var clicksNo = 0;
		var focusedIndx = null;
		var active = "";
		for(i=0;i<this.tabs.length;i++)
		{
			active = "";
			clicksNo = 0;
			var objDiv = getDiv(getObj("fixedTabsTable"),this.tabs[i].divName);
			if(this.tabs[i].divName == focusedTab)
			{
				if(trim(this.tabs[i].title," ")!="") active = " class='active'";
				eval(this.tabs[i].func+"()");
				//eval(master_less_for_dt());
				//master_less_for_dt();
				clicksNo = 1;
				focusedIndx = i;
				focusedTabIndx = parseInt(i)+1;
				objDiv.className = "tab-pane active";
			}
			else
			{
				objDiv.className = "tab-pane";
			}
			this.titleTbl+="<li "+active+"><a id='"+tabSep+this.tabs[i].divName+"' parentDiv='"+this.tabs[i].divName+"' href='javascript:;' onclick=\"clickTab(this, '"+i+"', '"+this.tabs[i].func+"')\" clicksNo="+clicksNo+">"+this.tabs[i].title+"</a></li>";
		}
		this.titleTbl+="</ul>";
		eval();
		return this.titleTbl;
	}

}


function getDiv(obj,searchId, p_tag)
{
	if(p_tag == null) p_tag = "div";
	var arr = getObjChildNodes(obj,p_tag);//.getElementsByTagName(p_tag);
	for(var i=0;i<arr.length;i++){
		//alert(arr[i].id+"\n"+searchId)
		if(arr[i].id == searchId)
			return arr[i];
	}
	return null;
}

function clickTab(obj, tabIndex, func)
{


	var tbl = getObj("navlist");
	var aftertTitleDiv  = obj;
	var beforeTitleDiv  = getDiv(tbl, findAttr(tbl,"selectedTab"),"a");
	var outerTabsTable   = getObj("fixedTabsTable");

	var aftertParentDiv  = getDiv(outerTabsTable,findAttr(obj,"parentDiv"));
	var sel = getDiv(tbl,findAttr(tbl,"selectedTab"),"a");
	var beforeParentDiv  = getDiv(outerTabsTable,findAttr(sel,"parentDiv"));
	if(aftertTitleDiv == beforeTitleDiv)
		return true;			// To not click on the actived tab.

	//if(!validate_hd()) return;
	//aftertTitleDiv.parentNode.className = "";
	//aftertParentDiv.className = "tab-pane";
	if(!validate_all_dt(true)) return false;
	// hide the displayed...
	var posLeft = beforeParentDiv.style.left;
	var posTop = beforeParentDiv.style.top;
	beforeTitleDiv.parentNode.className = "";
	beforeParentDiv.className = "tab-pane";
	
	var childDivs = getObjChildNodes(beforeParentDiv,"div");//.getElementsByTagName("DIV");
	for(var k=0;k<childDivs.length;k++){
		if(findAttr(childDivs[k],"oldId") == "sub_"+beforeParentDiv.id){
			childDivs[k].id = (findAttr(childDivs[k],"oldId") !=null) ? findAttr(childDivs[k],"oldId") : "";
			break;
		}
	}
  	


	// Display the hidden...
	aftertTitleDiv.parentNode.className = "active";
	aftertParentDiv.className = "tab-pane active";
	var childDivs = getObjChildNodes(aftertParentDiv,"div");//.getElementsByTagName("DIV");
	for(var k=0;k<childDivs.length;k++){
		if(childDivs[k].id == "sub_"+aftertParentDiv.id){
			childDivs[k].id = "root";
			break;
		}
	}

	setAttr(tbl,"selectedTab",obj.id);
	var len = tabSep.length;
	focusedTab = findAttr(tbl,"selectedTab").substr(len);
	setFormFocusTabValue(focusedTab);
	eval(func+"()");
	if(findAttr(obj,"clicksNo")==0){
		prevent_masterless(aftertParentDiv);
	}
	setAttr(obj,"clicksNo",parseInt(findAttr(obj,"clicksNo")) +1);
	focusedTabIndx = parseInt(tabIndex)+1;
	return true;
}


