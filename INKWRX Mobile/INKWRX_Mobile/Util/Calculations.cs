using FormTools.FormDescriptor;
using INKWRX_Mobile.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.Util
{
    class Calculations
    {
        private static Calculations instance = new Calculations();

        public static Calculations GetInstance()
        {
            return instance;
        }

        private bool Recalculating = false;
		public bool HoldCalculations { get; set; }
        private List<string> ActivatedFieldNameList;
        private List<CalcList> CalcListList;
        private HeaderStackLayout[] HeaderStackLayoutList;

        public void Recalculate(List<string> activatedFieldNameList, List<CalcList> calcListList, IEnumerable<HeaderStackLayout> headerStackLayoutList)
        {
            if (Recalculating || HoldCalculations)
            {
                System.Diagnostics.Debug.WriteLine("calc Recalculateing");
                return;
            }
            Recalculating = true;

            this.ActivatedFieldNameList = activatedFieldNameList;
            this.CalcListList = new List<CalcList>(calcListList);
            this.HeaderStackLayoutList = headerStackLayoutList.ToArray();
            if (this.CalcListList.Any())
            {
                this.ProcessCalc(this.CalcListList[0]);
            }
            else
            {
                Recalculating = false;
            }
        }

        private void ProcessCalc(CalcList calcList)
        {
            Task.Run(() =>
            {
                bool notActiviated = true;
                foreach (string activatedFieldName in ActivatedFieldNameList)
                {
                    if (calcList.Inputs.Contains(activatedFieldName))
                    {
                        notActiviated = false;
                        List<string> inputs = new List<string>(calcList.Inputs);
                        Dictionary<string, string> inputValueDictionary = new Dictionary<string, string>();

                        string expression = calcList.Descriptor.Calc;
                        foreach (HeaderStackLayout headerpanel in HeaderStackLayoutList)
                        {
                            foreach (IElementView field in headerpanel.FieldList)
                            {
                                string fieldName = field.RawDescriptor.FdtFieldName;
                                if (inputs.Contains(fieldName))
                                {

                                    string value = field.FieldValue;
                                    if (string.IsNullOrEmpty(value))
                                    {
                                        value = "0";
                                    }
                                    else if (value.Contains("."))
                                    {
                                        while (value.EndsWith("0"))
                                        {
                                            value = value.Substring(0, value.Length - 1);
                                        }
                                        if (value.EndsWith("."))
                                        {
                                            value = value.Remove(value.Length - 1);//remove . from end of value
                                        }
                                        else if (value.StartsWith("."))
                                        {
                                            value = "0" + value;//add 0 to the front of a number
                                        }
                                    }
                                    expression = expression.Replace("#" + fieldName + "#", value);
                                    
                                    //inputValueDictionary.Add(fieldName, field.FieldValue);
                                    inputs.Remove(fieldName);
                                    if (inputs.Count == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (inputs.Count == 0)
                            {
                                break;
                            }
                        }

                        // no repeating panels = no Sum() functions

                        RunCalculation(calcList, expression);
                        break;
                    }
                }

                if (notActiviated)
                {
                    this.CalcListList.Remove(calcList);

                    if (this.CalcListList.Count == 0)
                    {
                        Recalculating = false;
                    }
                    else
                    {
                        this.ProcessCalc(this.CalcListList[0]);
                    }
                }
            });
        }

        public void PredifinedCalc(CalcList calcList)
        {
            this.ActivatedFieldNameList = new List<string>();
            this.CalcListList = new List<CalcList> { calcList };
            this.RunCalculation(calcList, calcList.Descriptor.Calc);
        }

        private void RunCalculation(CalcList calcList, string expression)
        {
            double result = Calculator.Calculate(expression);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (calcList.FieldView is DecimalFieldView)
                {
                    ((DecimalFieldView)calcList.FieldView).FieldValue = result.ToString();
                }
                else
                {
                    ((ISOFieldView)calcList.FieldView).FieldValue = result.ToString();
                }

                ActivatedFieldNameList.Add(calcList.FieldName);

                this.CalcListList.Remove(calcList);

                if (this.CalcListList.Count == 0)
                {
                    Recalculating = false;
                }
                else
                {
                    this.ProcessCalc(this.CalcListList[0]);
                }
            });
        }
    }
}
