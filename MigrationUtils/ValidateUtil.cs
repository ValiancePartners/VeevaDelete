//-----------------------------------------------------------------------
// <copyright file="ValidateUtil.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Xml;
using System.Collections;

namespace TRUmigrate_1
{
	/// <summary>
	/// Summary description for ValidateUtil.
	/// 2.1 HOTFIX 1
	///		AK, 10/27/06, BUGAWARE #177
	///		AK, 10/27/06, BUGAWARE #179
	///	2.2 SP3
	///		AK, 6/21/07, BUGAWARE #222, pre-condition operators
	///		AK, 6/21/07, BUGAWARE #223, pre-condition operators
	/// </summary>
	public class ValidateUtil
	{
		public ValidateUtil()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//US, 4/18/07, BUGAWARE #204, make function public for use in filesystemconnectionutil filter selection
		//private bool EvaluateArrays(ArrayList arrParam1,ArrayList arrParam2, String op)
		public bool EvaluateArrays(ArrayList arrParam1,ArrayList arrParam2, String op)
		{
			String strParam1="";
			String strParam2="";
			bool conditionValid=true;
			op=op.ToUpper();
			
			//US, 4/25/07, BUGAWARE #207, make in Operator functional
			if (op.ToUpper().Equals("IN"))
				return EvalIn(arrParam1,arrParam2);
			//end of changes 4/25/07
			//AK, 6/21/07, BUGAWARE #223, add "not in" operator
			if (op.ToUpper().Equals("NOT IN"))
				return !EvalIn(arrParam1,arrParam2);
			//end changes by AK on 6/21/07
			if (arrParam2.Count==1)
			{
				strParam2=(String)arrParam2[0];
				for (int j=0;j<arrParam1.Count;j++)
				{
					strParam1=(String)arrParam1[j];
					conditionValid=Evaluate(strParam1,strParam2,op);
					if (conditionValid==false)
					{	
						return false;						
					}
				}
				return conditionValid;
			}
			else
			{
				if(! op.Equals("IN"))
				{
					throw new Exception("Value can only be a multi value for IN comparison");
				}
				return EvalIn(arrParam1,arrParam2);
			}					
		}

		private bool Evaluate(Object param1, Object param2, String operation)
		{
			bool result=false;
			operation=operation.ToUpper();
			switch(operation)
			{
				case "EQUAL":
					result=EvalEqual(param1,param2);
					break;
				case "NOT EQUAL":
					result=this.EvalNotEqual(param1,param2);
					break;
				case "GREATER OR EQUAL":
					result=this.EvalGreaterThanOrEqual(param1,param2);
					break;
				case "GREATER":
					result=this.EvalGreaterThan(param1,param2);
					break;
				case "LESS OR EQUAL":
					result=this.EvalLessThanOrEqual(param1,param2);
					break;
				case "LESS":
					result=this.EvalLessThan(param1,param2);
					break;
				case "LIKE":
					result=this.EvalLike(param1,param2);
					break;
				case "NOT LIKE":
					result=this.EvalNotLike(param1,param2);
					break;
				case "BEGINS WITH":
					result=this.EvalBeginsWith(param1,param2);
					break;
				case "ENDS WITH":
					result=this.EvalEndsWith(param1,param2);
					break;
				//US, 4/20/07, BUGAWARE #206, add "not like" operators for filesystem queries
				case "NOT BEGINS WITH":
					result=this.EvalNotBeginsWith(param1,param2);
					break;
				case "NOT ENDS WITH":
					result=this.EvalNotEndsWith(param1,param2);
					break;
				//end of changes 4/20/07
				//AK, 6/21/07, BUGAWARE #222, add "contains" and "does not contain" functions
				case "CONTAINS":
					result = this.EvalContains(param1, param2);
					break;
				case "DOES NOT CONTAIN":
					result = this.EvalNotContains(param1, param2);
					break;
				//end changes by AK on 6/21/07
				//AK, 6/21/07, BUGAWARE #223, add "does not begin with" and "does not end with" operators
				case "DOES NOT BEGIN WITH":
					result=this.EvalNotBeginsWith(param1,param2);
					break;
				case "DOES NOT END WITH":
					result=this.EvalNotEndsWith(param1,param2);
					break;
				//end changes by AK on 6/21/07

			}
			return result;			
		}

		//US, 4/23/07, BUGAWARE #207, change parameter to object type
		private bool EvalIn(ArrayList param1, ArrayList param2)
		//private bool EvalIn(Object p1, Object p2)
		{
		//	String param1 =  (String) p1;
		//	String param2 =  (String) p2;
			//end of changes 4/23/07

			//For now assume delimiter is a comma
			//Assume that param2 could be multivalue source attribute which is delimited by commma
			ArrayList compareValues=new ArrayList();
			bool isIn=false;

			foreach(Object param2Value in param2)
			{
				String currVal=(String)param2Value;
				String[] tmpArray=currVal.Split(",".ToCharArray());
				for(int i=0;i<tmpArray.Length;i++)
				{
					compareValues.Add(tmpArray[i]);
				}
			}

			foreach(Object param1Value in param1)
			{
				String currValue=(String)param1Value;
				if (compareValues.Contains(currValue))
				{
					isIn=true;
				}
				else
				{
					return false;
				}
			}
			return isIn;
		}

		private bool EvalEqual(Object param1, Object param2)
		{
			String myParam1=param1.ToString();
			String myParam2=param2.ToString();
			if (myParam1.Equals(param2))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		private bool EvalBeginsWith(Object param1, Object param2)
		{
			String myParam1=param1.ToString();
			String myParam2=param2.ToString();
			if (myParam1.StartsWith(myParam2))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		private bool EvalEndsWith(Object param1, Object param2)
		{
			String myParam1=param1.ToString();
			String myParam2=param2.ToString();
			if (myParam1.EndsWith(myParam2))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool EvalGreaterThan(Object param1, Object param2)
		{
			bool retVal =false;
			//Treat all params as double for precision
			
			//AK, 10/13/05, handle dates
			try 
			{
				DateTime dt1 = DateTime.Parse(param1.ToString());
				DateTime dt2 = DateTime.Parse(param2.ToString());
				return dt1>dt2;
			}
			catch
			{	//not a date, try numbers
			}
			//end changes by AK, 10/13/05

			try
			{
				double myparam1=double.Parse(param1.ToString());
				double myparam2=double.Parse(param2.ToString());
				retVal=myparam1>myparam2;
			}
			catch (Exception e)
			{
				throw new Exception("Parameters invalid for Greater Than comparison." + e.Message);
			}
			return retVal;
		}

		private bool EvalGreaterThanOrEqual(Object param1, Object param2)
		{
			bool retVal =false;
			//Treat all params as double for precision
			
			//AK, 10/13/05, handle dates
			try 
			{
				DateTime dt1 = DateTime.Parse(param1.ToString());
				DateTime dt2 = DateTime.Parse(param2.ToString());
				return dt1>=dt2;
			}
			catch
			{	//not a date, try numbers
			}
			//end changes by AK, 10/13/05

			try
			{
				double myparam1=double.Parse(param1.ToString());
				double myparam2=double.Parse(param2.ToString());
				retVal=myparam1>=myparam2;
			}
			catch (Exception e)
			{
				throw new Exception("Parameters invalid for Greater Than comparison." + e.Message);
			}
			return retVal;
		}

		private bool EvalLessThan(Object param1, Object param2)
		{
			bool retVal =false;
			//Treat all params as double for precision
			
			//AK, 10/13/05, handle dates
			try 
			{
				DateTime dt1 = DateTime.Parse(param1.ToString());
				DateTime dt2 = DateTime.Parse(param2.ToString());
				return dt1<dt2;
			}
			catch
			{	//not a date, try numbers
			}
			//end changes by AK, 10/13/05

			try
			{
				double myparam1=double.Parse(param1.ToString());
				double myparam2=double.Parse(param2.ToString());
				retVal=myparam1<myparam2;
			}
			catch (Exception e)
			{
				throw new Exception("Parameters invalid for Greater Than comparison." + e.Message);
			}
			return retVal;
		}

		private bool EvalLessThanOrEqual(Object param1, Object param2)
		{
			bool retVal =false;
			//Treat all params as double for precision
			
			//AK, 10/13/05, handle dates
			try 
			{
				DateTime dt1 = DateTime.Parse(param1.ToString());
				DateTime dt2 = DateTime.Parse(param2.ToString());
				return dt1<=dt2;
			}
			catch
			{	//not a date, try numbers
			}
			//end changes by AK, 10/13/05

			
			try
			{
				double myparam1=double.Parse(param1.ToString());
				double myparam2=double.Parse(param2.ToString());
				retVal=myparam1<=myparam2;
			}
			catch (Exception e)
			{
				throw new Exception("Parameters invalid for Greater Than comparison." + e.Message);
			}
			return retVal;
		}

		private bool EvalNotEqual(Object param1, Object param2)
		{
			return !EvalEqual(param1, param2);
		}

		//AK, 6/21/07, BUGAWARE #222, change this to "contains" function, "like" function will check the pattern
		//private bool EvalLike(Object param1, Object param2)
		private bool EvalContains(Object param1, Object param2)
		{
			String myParam1=param1.ToString();
			String myParam2=param2.ToString();
			//AK, 10/27/06, BUGAWARE #177
			//use IndexOf instead of regular expression match
			if (myParam1.IndexOf(myParam2)>-1)
				return true;
			else
				return false;
			/*myParam2=myParam2.Replace("*",@"(\w){0,}");
			myParam2=myParam2.Replace("?",@"(\w){1}");
			myParam2="^" + myParam2 + @"\z";
			return System.Text.RegularExpressions.Regex.Match(myParam1,myParam2).Success;	*/	
			//end changes by AK on 10/27/06
		}

		//AK, 6/21/07, BUGAWARE #222, change this to "does not contain" function, "not like" function will check the pattern
		//private bool EvalNotLike(Object param1, Object param2)
		private bool EvalNotContains(Object param1, Object param2)
		{
			//AK, 6/21/07, BUGAWARE #222, function named changed
			return !EvalContains(param1,param2);
			//return !EvalLike(param1,param2);
		}
		//end changes by AK on 6/21/07

		//AK, 6/21/07, BUGAWARE #222, add EvalLike function to verify pattern
		private bool EvalLike(Object param1, Object param2)
		{
			String myParam1=param1.ToString();
			String pattern=param2.ToString();
			pattern=pattern.Replace("*",@"[\w]{0,}");
			pattern=pattern.Replace("?",@"[\w]{1}");
			bool rval =  System.Text.RegularExpressions.Regex.Match(myParam1,pattern).Success;
			return rval;
		}
		//end changes by AK on 6/21/07

		//AK, 6/21/07, BUGAWARE #222, add EvalNotLike function to verify pattern
		private bool EvalNotLike(Object param1, Object param2)
		{
			return !EvalLike(param1, param2);
		}
		//end changes by AK on 6/21/07

		//US, 4/20/07, bugaware #206, add  "not like" filters for query
		private bool EvalNotBeginsWith(Object param1, Object param2)
		{
			return !EvalBeginsWith(param1,param2);
		}
		private bool EvalNotEndsWith(Object param1, Object param2)
		{
			return !EvalEndsWith(param1,param2);
		}
		//end of changes 4/20/07

		private bool EvalAnd(bool param1, bool param2)
		{
			bool retVal=param1 && param2;
			return retVal;
		}

		private bool EvalOr(bool param1, bool param2)
		{
			bool retVal=param1 && param2;
			return retVal;
		}

		private bool EvalNot(bool param1)
		{
			bool retVal=!param1;
			return param1;
		}


	}
}
