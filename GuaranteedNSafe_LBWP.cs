/*
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using LightInfocon.Data.LightBaseProvider;

namespace GuaranteedNSafe_LBWP
{

    class GuaranteedNSafe_LBWP
    {

        /// <summary>
        /// Guarda a string de conexão do LightBase Provider! By Questor
        /// </summary>
        private string _LBWConnectionString = "";

        public string LBWConnectionString
        {
            get { return _LBWConnectionString; }
        }

        /// <summary>
        /// Guarda a conexão LightBase principal! By Questor
        /// </summary>
        private LightBaseConnection _LBWConnection = null;

        private LightBaseConnection LBWConnection
        {
            get
            {
                if (_LBWConnection != null)
                {
                    if (_LBWConnection.State != ConnectionState.Open)
                    {
                        _LBWConnection.Open();
                    }
                }
                return _LBWConnection;
            }
        }

        /// <summary>
        /// Guarda a conexão LightBase usada nos processos seguros/de confirmação! By Questor
        /// </summary>
        private LightBaseConnection _LBWConnectionToTests = null;

        private LightBaseConnection LBWConnectionToTests
        {
            get
            {
                if (_LBWConnectionToTests != null)
                {
                    if (_LBWConnectionToTests.State != ConnectionState.Open)
                    {
                        _LBWConnectionToTests.Open();
                    }
                }
                return _LBWConnectionToTests;
            }
        }

        /// <summary>
        /// Guarda a última query usada! By Questor
        /// </summary>
        private string _LastSearchPerformed = "";

        public string LastSearchPerformed
        {
            get { return _LastSearchPerformed; }
        }

        /// <summary>
        /// Guarda o nome do último grupo multivalorado usado na recuperação de grupos multivalorados de forma segura! By Questor
        /// </summary>
        private string _LastMultiValuedGroupName = "";

        public string LastMultiValuedGroupName
        {
            get { return _LastMultiValuedGroupName; }
        }

        /// <summary>
        /// Guarda o nome do último campo de conferência usado na recuperação de grupos multivalorados de forma segura! By Questor
        /// </summary>
        private string _LastTrustworthyField = "";

        public string LastTrustworthyField
        {
            get { return _LastTrustworthyField; }
        }

        /// <summary>
        /// Guarda a última query de confirmação usada na recuperação de grupos multivalorados de forma segura! By Questor
        /// </summary>
        private string _LastConfirmationQuery = "";

        public string LastConfirmationQuery
        {
            get { return _LastConfirmationQuery; }
        }

        //private bool _SecureUpdateEnabled = false;

        //public bool SecureUpdateEnabled
        //{
        //    get { return _SecureUpdateEnabled; }
        //}

        //private DataTable _LastDataTableSetted = null;

        //public DataTable LastDataTableSetted
        //{
        //    get { return _LastDataTableSetted; }
        //}

        /// <summary>
        /// Guarda o DataAdapter atual! By Questor
        /// </summary>
        private LightBaseConnectedDataReader _LBWDataReader = null;

        private LightBaseConnectedDataReader LBWDataReader
        {
            get { return _LBWDataReader; }
        }

        /// <summary>
        /// Guarda o command atual! By Questor
        /// </summary>
        private LightBaseCommand _LBWCommand = null;

        private LightBaseCommand LBWCommand
        {
            get { return _LBWCommand; }
        }

        /// <summary>
        /// Guarda o DataAdapter atual! By Questor
        /// </summary>
        private LightBaseDataAdapter _LBWLightBaseDataAdapter = null;

        private LightBaseDataAdapter LBWLightBaseDataAdapter
        {
            get { return _LBWLightBaseDataAdapter; }
        }

        /// <summary>
        /// Guarda a posição do DataReader atual By Questor
        /// </summary>
        private int _LBWDataReaderPositionCounter = 0;

        public int LBWDataReaderPositionCounter
        {
            get { return _LBWDataReaderPositionCounter; }
        }

        //private List<DataRow> _GetLBWDataTable_N_Manipulate_List = null;

        //private List<DataRow> GetLBWDataTable_N_Manipulate_List
        //{
        //    get { return _GetLBWDataTable_N_Manipulate_List; }
        //}

        //private List<String> _GetLBWDataTable_N_Manipulate_List = null;

        //private List<String> GetLBWDataTable_N_Manipulate_List
        //{
        //    get { return _GetLBWDataTable_N_Manipulate_List; }
        //}

        /// <summary>
        /// Valor do grupo multivalorado atual! By Questor
        /// </summary>
        private string _IdFieldForThisMultivaluedSet_Now = "";

        public string IdFieldForThisMultivaluedSet_Now
        {
            get { return _IdFieldForThisMultivaluedSet_Now; }
        }

        /// <summary>
        /// Seta uma instância do "GuaranteedNSafe_LBWP"! By Questor
        /// </summary>
        /// <param name="LBWConnectionString_"></param>
        public GuaranteedNSafe_LBWP(string LBWConnectionString_)
        {
            _LBWConnectionString = LBWConnectionString_;
            _LBWConnection = new LightBaseConnection(LBWConnectionString_);
            _LBWConnectionToTests = new LightBaseConnection(LBWConnectionString_);
        }

        /// <summary>
        /// Fecha as conecções! By Questor
        /// </summary>
        public void CloseLBWConnections()
        {
            LBWConnection.Close();
            LBWConnectionToTests.Close();
        }

        /// <summary>
        /// Seta um comando de forma segura e em codificação compatível! By Questor
        /// </summary>
        /// <param name="commandToPerform"></param>
        public void SetLBWCommand(string commandToPerform)
        {
            string operation = GetFirstFromSplit(commandToPerform).Trim().ToUpper();
            if (operation == "INSERT" || operation == "UPDATE")
            {
                commandToPerform = LBWEncoding(commandToPerform);
            }
            _LBWCommand = new LightBaseCommand(commandToPerform, LBWConnection);
            _LastSearchPerformed = commandToPerform;
        }

        /// <summary>
        /// Adiciona um parâmetros ao comando setado. Isso evita a existência de caracteres na query a que a tornem inválida! By Questor
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void AddLightBaseParameter(string parameterName, object value)
        {
            if (value is string)
            {
                LBWCommand.Parameters.Add(new LightBaseParameter(parameterName, LBWEncoding(value.ToString())));
                return;
            }
            LBWCommand.Parameters.Add(new LightBaseParameter(parameterName, value));
        }

        ///// <summary>
        ///// Adiciona um parâmetros ao comando setado. Isso evita a existência de caracteres na query a que a tornem inválida! By Questor
        ///// </summary>
        ///// <param name="parameterName"></param>
        ///// <param name="value"></param>
        //private void AddLightBaseParameter(string parameterName, object value)
        //{
        //    LBWCommand.Parameters.Add(new LightBaseParameter(parameterName, value));
        //}


        /// <summary>
        /// Método auxiliar da classe. By Questor
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private string GetFirstFromSplit(string strInput)
        {
            strInput = strInput.Trim();
            var i = strInput.IndexOf(" ");
            return i == -1 ? strInput : strInput.Substring(0, i);
        }

        /// <summary>
        /// Encoda os valores de um comando em formato compatível com o LightBase e evita caracteres inválidos no banco! By Questor
        /// </summary>
        /// <param name="strToConvert"></param>
        /// <returns></returns>
        public static string LBWEncoding(string strToConvert)
        {
            Encoding UTF8 = Encoding.UTF8;
            //Note: ISO-8859-1 = ANSI! By Questor
            //Encoding newEnc = Encoding.GetEncoding("ISO-8859-1");
            Encoding newEnc = Encoding.GetEncoding(28591);
            byte[] UTF8Bytes = newEnc.GetBytes(strToConvert);
            byte[] newEncBytes = Encoding.Convert(newEnc, UTF8, UTF8Bytes);
            char[] newEncChars = new char[UTF8.GetCharCount(newEncBytes, 0, newEncBytes.Length)];
            UTF8.GetChars(newEncBytes, 0, newEncBytes.Length, newEncChars, 0);
            return new string(newEncChars);
        }

        /// <summary>
        /// Excuta a query setada no Command de forma segura! By Questor
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQueryLBWCommand()
        {
            return LBWCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Obtém a posição atual do DataReader atual! By Questor
        /// </summary>
        public int CountLBWDataReader()
        {
            return LBWDataReader.Count;
        }


        // Note: Está aqui apenas por segurança, será removido na próxima versão! By Questor
        //private int countLBWDataReaderOneNeededInteractionsLimit = 25;
        //private int countLBWDataReaderOneNeededInteractionsLimitCounter = 0;
        //private int countLBWDataReaderOneNeededReturn = 0;
        ///// <summary>
        ///// Esse método é útil quando se quer verificar se um registro está duplicado, pois garante que a pesquisa foi 
        ///// executada corretamente pelo LBWP quando o resultado esperado é que o banco encontre apenas um registro com 
        ///// a query executada! By Questor
        ///// </summary>
        //public int CountLBWDataReader_Secure()
        //{
        //    if (countLBWDataReaderOneNeededInteractionsLimitCounter == 0)
        //    {
        //        countLBWDataReaderOneNeededReturn = 0;
        //    }
        //    countLBWDataReaderOneNeededReturn = LBWDataReader.Count;
        //    if(countLBWDataReaderOneNeededReturn > 1 && countLBWDataReaderOneNeededInteractionsLimitCounter < countLBWDataReaderOneNeededInteractionsLimit)
        //    {
        //        CloseLBWDataReader();
        //        SetLBWCommand(LastSearchPerformed);
        //        SetLBWDataReader();
        //        countLBWDataReaderOneNeededInteractionsLimitCounter++;
        //        CountLBWDataReader_Secure();
        //    }
        //    countLBWDataReaderOneNeededInteractionsLimitCounter = 0;
        //    return countLBWDataReaderOneNeededReturn;
        //}


        /// <summary>
        /// Fecha o DataReader atual! By Questor
        /// </summary>
        public void CloseLBWDataReader()
        {
            if (_LBWDataReader != null)
            {
                if (!_LBWDataReader.IsClosed)
                {
                    _LBWDataReader.Close();
                }
            }
        }

        // ToDo: Tratar erros LBW? By Questor
        /// <summary>
        /// Seta um DataReader de forma segura! By Questor
        /// </summary>
        public void SetLBWDataReader()
        {


            //Note: O método "CloseLBWDataReader()" é chamado para garantir que o reader em "_LBWDataReader" se já existir esteja fechado, pois 
            //Note: caso contrário pode ocorrer um bug no qual o novo reader a ser setado por "SetLBWDataReader()" ficar com um lista de ocorrência 
            //Note: errada (normalmente a lista física). By Questor
            CloseLBWDataReader();
            
            _LBWDataReader = default(LightBaseConnectedDataReader);
            _LBWDataReaderPositionCounter = 0;

            if (LBWCommand != null)
            {

                _LBWDataReader = (LightBaseConnectedDataReader)LBWCommand.ExecuteReader();

                //Todo: Tratar esse e outros incluir limite de loopings! By Questor
                //LightInfocon.Data.LightBaseProvider.LightBaseException was unhandled
                //Message="LightBase Error: Erro em alocacao de memoria"
                //Source="LightInfocon.Data.LightBaseProvider"
                //ErrorCode=-302
                //StackTrace:
                //     at LightInfocon.Data.LightBaseProvider.LightBaseConnection.OpenBase(String sBaseName, String sQuery, Boolean FilterChildren)
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteSelectCommand(SelectCommand Code)
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteCompiledCode(CompiledCommandCollection Code)
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteCommand()
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteReader(CommandBehavior behavior)
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteReader()
                //     at GuaranteedNSafe_LBWP.GuaranteedNSafe_LBWP.SetLBWDataReader() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\GuaranteedNSafe_LBWP.cs:line 119
                //     at GuaranteedNSafe_LBWP.ManipulateTheDatabase.AdjustDataBases() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\ManipulateTheDatabase.cs:line 43
                //     at GuaranteedNSafe_LBWP.MainBusiness.MainBusinessExecuter() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\MainBusiness.cs:line 38
                //     at DoCControl_Adjuster.Program.Main(String[] args) in Z:\E\DEV\DoCControl_Adjuster\DoCControl_Adjuster\Program.cs:line 15
                //     at System.AppDomain._nExecuteAssembly(Assembly assembly, String[] args)
                //     at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)
                //     at Microsoft.VisualStudio.HostingProcess.HostProc.RunUsersAssembly()
                //     at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
                //     at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
                //     at System.Threading.ThreadHelper.ThreadStart()
                //InnerException: 

                //Note: Clausula top não suportada! By Questor
                //System.InvalidCastException was unhandled
                //Message="Unable to cast object of type 'LightInfocon.Data.LightBaseProvider.LightBaseViewDataReader' to type 'LightInfocon.Data.LightBaseProvider.LightBaseConnectedDataReader'."
                //Source="GuaranteedNSafe_LBWP"
                //StackTrace:
                //     at GuaranteedNSafe_LBWP.GuaranteedNSafe_LBWP.SetLBWDataReader() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\GuaranteedNSafe_LBWP.cs:line 144
                //     at GuaranteedNSafe_LBWP.ManipulateTheDatabase.AdjustDataBases() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\ManipulateTheDatabase.cs:line 625
                //     at GuaranteedNSafe_LBWP.MainBusiness.MainBusinessExecuter() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\MainBusiness.cs:line 38
                //     at DoCControl_Adjuster.Program.Main(String[] args) in Z:\E\DEV\DoCControl_Adjuster\DoCControl_Adjuster\Program.cs:line 15
                //     at System.AppDomain._nExecuteAssembly(Assembly assembly, String[] args)
                //     at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)
                //     at Microsoft.VisualStudio.HostingProcess.HostProc.RunUsersAssembly()
                //     at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
                //     at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
                //     at System.Threading.ThreadHelper.ThreadStart()
                //InnerException: 

                //Todo: Tratar esse e outros incluir limite de loopings! By Questor
                //LightInfocon.Data.LightBaseProvider.LightBaseException was unhandled
                //Message="LightBase Error: Não houve erro"
                //Source="LightInfocon.Data.LightBaseProvider"
                //ErrorCode=0
                //StackTrace:
                //     at LightInfocon.Data.LightBaseProvider.LightBaseConnection.OpenBase(String sBaseName, String sQuery, Boolean FilterChildren)
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteSelectCommand(SelectCommand Code)
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteCompiledCode(CompiledCommandCollection Code)
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteCommand()
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteReader(CommandBehavior behavior)
                //     at LightInfocon.Data.LightBaseProvider.LightBaseCommand.ExecuteReader()
                //     at GuaranteedNSafe_LBWP.GuaranteedNSafe_LBWP.SetLBWDataReader() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\GuaranteedNSafe_LBWP.cs:line 145
                //     at GuaranteedNSafe_LBWP.ManipulateTheDatabase.AdjustDataBases() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\ManipulateTheDatabase.cs:line 1078
                //     at GuaranteedNSafe_LBWP.MainBusiness.MainBusinessExecuter() in Z:\E\DEV\DoCControl_Adjuster\GuaranteedNSafe_LBWP\MainBusiness.cs:line 38
                //     at DoCControl_Adjuster.Program.Main(String[] args) in Z:\E\DEV\DoCControl_Adjuster\DoCControl_Adjuster\Program.cs:line 15
                //     at System.AppDomain._nExecuteAssembly(Assembly assembly, String[] args)
                //     at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)
                //     at Microsoft.VisualStudio.HostingProcess.HostProc.RunUsersAssembly()
                //     at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
                //     at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
                //     at System.Threading.ThreadHelper.ThreadStart()
                //InnerException: 

            }
        }


        // ToDo: Tratar erros LBW? By Questor
        /// <summary>
        /// Lê a próxima posição do DataReader e guarda a o valor desta posição!
        /// </summary>
        /// <returns></returns>
        public bool ReadSafe()
        {
            if (_LBWDataReader.Read())
            {
                _LBWDataReaderPositionCounter++;
                return true;
            }
            return false;
        }


        // ToDo: Tratar erros LBW? By Questor
        /// <summary>
        /// Obtém o valor de um campo do usando DataReader instanciado! By Questor
        /// </summary>
        /// <param name="fieldToGet"></param>
        /// <returns></returns>
        public object GetLBWDataReaderField(string fieldToGet)
        {
            return LBWDataReader[fieldToGet];
        }

        /// <summary>
        /// Obtém um datatable com os valores multivalorados e permite a sua atualização de forma simples e segura depois! By Questor
        /// </summary>
        /// <param name="multiValuedGroupName"></param>
        /// <param name="trustworthyField"></param>
        /// <param name="confirmationQuery"></param>
        /// <param name="DataTableToSet"></param>
        public void GetLBWDataTable_Secure(string multiValuedGroupName, string trustworthyField, string confirmationQuery, ref DataTable DataTableToSet)
        {
            _LastMultiValuedGroupName = multiValuedGroupName;
            _LastTrustworthyField = trustworthyField;
            _LastConfirmationQuery = confirmationQuery;
            GetLBWDataTable(multiValuedGroupName, trustworthyField, confirmationQuery, ref DataTableToSet);
            //_LastDataTableSetted = DataTableToSet;
        }

        /// <summary>
        /// Recarrega uma datatable com informações adivindas do banco. É útil nos casos em que queremos saber a posição de determinada repetição de 
        /// um campo multivalorado quando o processo de update é incerto, ous seja, nos casos onde o banco atualiza determinada repetição em sua posição 
        /// e/ou atualiza determinada repetição colocando-a na última posição daquele grupo multivalorado. Deve ser usado em conjunto com GetLBWDataTable_Secure. 
        /// By Questor
        /// </summary>
        /// <param name="DataTableHolder0"></param>
        public void GetLBWDataTable_Secure_UpdateLastDataTable(ref DataTable DataTableToSet)
        {
            SetLBWCommand(LastConfirmationQuery);
            SetLBWDataReader();
            ReadSafe();
            GetLBWDataTable(LastMultiValuedGroupName, LastTrustworthyField, LastConfirmationQuery, ref DataTableToSet);
        }

        private bool GetLBWDataTableSearchAgain = false;
        int getLBWDataTableInteractionsLimit = 25;
        int getLBWDataTableInteractionsLimitCounter = 0;
        /// <summary>
        /// Obtém um grupo multivalorado de um registro de forma segura. Traz todos campos e todas as linhas. By Questor
        /// </summary>
        /// <param name="multiValuedGroupName"></param>
        /// <param name="trustworthyField"></param>
        /// <param name="confirmationQuery"></param>
        /// <param name="DataTableToSet"></param>
        public void GetLBWDataTable(string multiValuedGroupName, string trustworthyField, string confirmationQuery, ref DataTable DataTableToSet)
        {
            int rowsCount = 0;
            string trustworthyFieldValue = "";
            int getLBWDataTableCountController = 0;
            bool GetLBWDataTableSet = false;
            Object objectHolder = null;
            DataTable objectHolderDataTable = null;
            Object[] objectHolderObjectArray = null;
            LightBaseConnectedDataReader GetLBWDataTableLBWDataReader = null;
            LightBaseConnectedDataReader LightBaseConnectedDataReaderHolder = null;
            if (GetLBWDataTableSearchAgain)
            {
                if (string.IsNullOrEmpty(confirmationQuery))
                {
                    int getLBWDataTableRowsCounterController = 0;
                    LightBaseConnectedDataReaderHolder = (LightBaseConnectedDataReader)new LightBaseCommand(LastSearchPerformed, LBWConnectionToTests).ExecuteReader();
                    while (LBWDataReaderPositionCounter > getLBWDataTableRowsCounterController)
                    {
                        LightBaseConnectedDataReaderHolder.Read();
                        getLBWDataTableRowsCounterController++;
                    }
                }
                else
                {
                    LightBaseConnectedDataReaderHolder = (LightBaseConnectedDataReader)new LightBaseCommand(confirmationQuery, LBWConnectionToTests).ExecuteReader();
                    LightBaseConnectedDataReaderHolder.Read();
                }
                GetLBWDataTableLBWDataReader = LightBaseConnectedDataReaderHolder;
            }
            else
            {
                GetLBWDataTableLBWDataReader = LBWDataReader;
            }

            try
            {
                if (GetLBWDataTableLBWDataReader.Count >= 1)
                {
                    objectHolder = GetLBWDataTableLBWDataReader[multiValuedGroupName];
                    if (objectHolder != null)
                    {
                        if (objectHolder is DataTable)
                        {
                            objectHolderDataTable = (DataTable)objectHolder;
                            if (objectHolderDataTable.Rows.Count > 0)
                            {
                                foreach (DataRow dataTableRow in objectHolderDataTable.Rows)
                                {
                                    if (!string.IsNullOrEmpty(trustworthyField))
                                    {
                                        trustworthyFieldValue = dataTableRow[trustworthyField].ToString();
                                    }
                                    getLBWDataTableCountController++;
                                }

                            }
                        }
                        if (objectHolder is Object[])
                        {
                            objectHolderObjectArray = (Object[])objectHolder;
                            if (objectHolderObjectArray.Length > 0)
                            {
                                foreach (Object objectValue in objectHolderObjectArray)
                                {
                                    if (!string.IsNullOrEmpty(trustworthyField))
                                    {
                                        trustworthyFieldValue = objectValue.ToString();
                                    }
                                    getLBWDataTableCountController++;
                                }

                            }
                        }
                        //Note: To debug! By Questor
                        //if (getLBWDataTableInteractionsLimitCounter == 25)
                        //{
                        //    string erro = "";
                        //}
                        if (!string.IsNullOrEmpty(confirmationQuery))
                        {
                            GetLBWDataTableRowsCounterWorkAround(multiValuedGroupName, confirmationQuery, ref rowsCount);
                            //Note: To debug! By Questor
                            //if(getLBWDataTableCountController == 22 && rowsCount == 22)
                            //{
                            //    string test1 = "";
                            //}
                            if (getLBWDataTableCountController != rowsCount &&
                                getLBWDataTableInteractionsLimitCounter <= getLBWDataTableInteractionsLimit)
                            {
                                if (LightBaseConnectedDataReaderHolder != null)
                                {
                                    if (!LightBaseConnectedDataReaderHolder.IsClosed)
                                    {
                                        LightBaseConnectedDataReaderHolder.Close();
                                    }
                                }
                                getLBWDataTableInteractionsLimitCounter++;
                                GetLBWDataTableSet = true;
                                GetLBWDataTableSearchAgain = true;
                                GetLBWDataTable(multiValuedGroupName, trustworthyField, confirmationQuery,
                                                ref DataTableToSet);
                                return;
                            }
                        }
                        if (!string.IsNullOrEmpty(trustworthyField))
                        {
                            if (string.IsNullOrEmpty(trustworthyFieldValue) && getLBWDataTableCountController > 0 &&
                                getLBWDataTableInteractionsLimitCounter <= getLBWDataTableInteractionsLimit)
                            {
                                if (LightBaseConnectedDataReaderHolder != null)
                                {
                                    if (!LightBaseConnectedDataReaderHolder.IsClosed)
                                    {
                                        LightBaseConnectedDataReaderHolder.Close();
                                    }
                                }
                                getLBWDataTableInteractionsLimitCounter++;
                                GetLBWDataTableSet = true;
                                GetLBWDataTableSearchAgain = true;
                                GetLBWDataTable(multiValuedGroupName, trustworthyField, confirmationQuery,
                                                ref DataTableToSet);
                                return;
                            }
                        }
                    }
                }
            }
            catch (LightBaseException ex)
            {
                if ((ex.Message == "Não houve erro" || ex.Message == "Referência a uma repetição que não existe") && getLBWDataTableInteractionsLimitCounter <= getLBWDataTableInteractionsLimit)
                {
                    if (LightBaseConnectedDataReaderHolder != null)
                    {
                        if (!LightBaseConnectedDataReaderHolder.IsClosed)
                        {
                            LightBaseConnectedDataReaderHolder.Close();
                        }
                    }
                    getLBWDataTableInteractionsLimitCounter++;
                    GetLBWDataTableSet = true;
                    GetLBWDataTableSearchAgain = true;
                    GetLBWDataTable(multiValuedGroupName, trustworthyField, confirmationQuery, ref DataTableToSet);
                }
            }
            catch (Exception ex)
            {
                if ((ex.Message == "Attempted to read or write protected memory. This is often an indication that other memory is corrupt." || ex.Message == "Tentativa de leitura ou gravação em memória protegida. Normalmente, isso é uma indicação de que outra memória está danificada.") && getLBWDataTableInteractionsLimitCounter <= getLBWDataTableInteractionsLimit)
                {
                    if (LightBaseConnectedDataReaderHolder != null)
                    {
                        if (!LightBaseConnectedDataReaderHolder.IsClosed)
                        {
                            LightBaseConnectedDataReaderHolder.Close();
                        }
                    }
                    getLBWDataTableInteractionsLimitCounter++;
                    GetLBWDataTableSet = true;
                    GetLBWDataTableSearchAgain = true;
                    GetLBWDataTable(multiValuedGroupName, trustworthyField, confirmationQuery, ref DataTableToSet);
                }
            }

            //Note: Essa guambiarra serve para evitar o efeito colateral da chama recursiva! By Questor
            if (!GetLBWDataTableSet)
            {
                GetLBWDataTableSearchAgain = false;
                getLBWDataTableInteractionsLimitCounter = 0;

                if (objectHolderDataTable != null)
                {
                    DataTableToSet = objectHolderDataTable;
                }
                else if (objectHolderObjectArray != null)
                {
                    DataTableToSet = GetDataTableFromObjectArray(multiValuedGroupName, objectHolderObjectArray);
                }
            }

            if (LightBaseConnectedDataReaderHolder != null)
            {
                if (!LightBaseConnectedDataReaderHolder.IsClosed)
                {
                    LightBaseConnectedDataReaderHolder.Close();
                }
            }

        }


        //Note: O objetivo desse método é manter a compatibilidade com o objeto DataTable em qualquer caso! By Questor
        private DataTable GetDataTableFromObjectArray(string multiValuedGroupName, Object[] LBWDataReaderMultiValuedGroupName)
        {
            DataTable dataTableHolder = new DataTable();
            dataTableHolder.TableName = multiValuedGroupName;
            dataTableHolder.Columns.Add(multiValuedGroupName, typeof(Object));
            foreach (Object ObjectField in LBWDataReaderMultiValuedGroupName)
            {
                dataTableHolder.Rows.Add(ObjectField);
            }
            return dataTableHolder;
        }

        #region Converting ObjectArray to Datatable! By Questor
        private DataTable ConvertToDataTable(Object[] array)
        {
            PropertyInfo[] properties = array.GetType().GetElementType().GetProperties();
            DataTable dt = CreateDataTable(properties);
            if (array.Length != 0)
            {
                foreach (object o in array)
                    FillData(properties, dt, o);
            }
            return dt;
        }

        private DataTable CreateDataTable(PropertyInfo[] properties)
        {
            DataTable dt = new DataTable();
            DataColumn dc = null;
            foreach (PropertyInfo pi in properties)
            {
                dc = new DataColumn();
                dc.ColumnName = pi.Name;
                dc.DataType = pi.PropertyType;
                dt.Columns.Add(dc);
            }
            return dt;
        }

        private void FillData(PropertyInfo[] properties, DataTable dt, Object o)
        {
            DataRow dr = dt.NewRow();
            foreach (PropertyInfo pi in properties)
            {
                dr[pi.Name] = pi.GetValue(o, null);
            }
            dt.Rows.Add(dr);
        }

        #endregion


        int GetLBWDataTableRowsCounterWorkAroundInteractionsLimit = 25;
        int GetLBWDataTableRowsCounterWorkAroundLimitCounter = 0;
        /// <summary>
        /// Faz parte das regras de recuperação de grupos multivalortados de forma segura! By Questor
        /// </summary>
        /// <param name="multiValuedGroupName"></param>
        /// <param name="confirmationQuery"></param>
        /// <param name="rowsCount"></param>
        private void GetLBWDataTableRowsCounterWorkAround(string multiValuedGroupName, string confirmationQuery, ref int rowsCount)
        {

            int getLBWDataTableRowsCounterController = 0;
            int GetLBWDataTableRowsCounterWorkAroundOutPut = 0;
            Object objectHolder = null;
            LightBaseConnectedDataReader LightBaseConnectedDataReaderHolder = null;
            if (string.IsNullOrEmpty(confirmationQuery))
            {
                LightBaseConnectedDataReaderHolder = (LightBaseConnectedDataReader)new LightBaseCommand(LastSearchPerformed, LBWConnectionToTests).ExecuteReader();
                while (LBWDataReaderPositionCounter > getLBWDataTableRowsCounterController)
                {
                    LightBaseConnectedDataReaderHolder.Read();
                    getLBWDataTableRowsCounterController++;
                }
            }
            else
            {
                LightBaseConnectedDataReaderHolder = (LightBaseConnectedDataReader)new LightBaseCommand(confirmationQuery, LBWConnectionToTests).ExecuteReader();
                LightBaseConnectedDataReaderHolder.Read();
            }

            bool blockRowsCountSet = false;
            try
            {
                if (LightBaseConnectedDataReaderHolder.Count > 0)
                {
                    objectHolder = LightBaseConnectedDataReaderHolder[multiValuedGroupName];
                    if (objectHolder != null)
                    {
                        if (LightBaseConnectedDataReaderHolder[multiValuedGroupName] is DataTable)
                        {
                            GetLBWDataTableRowsCounterWorkAroundOutPut = ((DataTable)objectHolder).Rows.Count;
                        }
                        else if (LightBaseConnectedDataReaderHolder[multiValuedGroupName] is Object[])
                        {
                            GetLBWDataTableRowsCounterWorkAroundOutPut = ((Object[])objectHolder).Length;
                        }
                    }
                }
                //Note: To debug! By Questor
                //if (GetLBWDataTableRowsCounterWorkAroundLimitCounter == 25)
                //{
                //    string erro = "";
                //}
            }
            catch (LightBaseException ex)
            {
                if ((ex.Message == "Não houve erro" || ex.Message == "Referência a uma repetição que não existe") && GetLBWDataTableRowsCounterWorkAroundLimitCounter <= GetLBWDataTableRowsCounterWorkAroundInteractionsLimit)
                {
                    if (LightBaseConnectedDataReaderHolder != null)
                    {
                        if (!LightBaseConnectedDataReaderHolder.IsClosed)
                        {
                            LightBaseConnectedDataReaderHolder.Close();
                        }
                    }
                    GetLBWDataTableRowsCounterWorkAroundLimitCounter++;
                    blockRowsCountSet = true;
                    GetLBWDataTableRowsCounterWorkAround(multiValuedGroupName, confirmationQuery, ref rowsCount);
                }
            }
            catch (Exception ex)
            {
                if ((ex.Message == "Attempted to read or write protected memory. This is often an indication that other memory is corrupt." || ex.Message == "Tentativa de leitura ou gravação em memória protegida. Normalmente, isso é uma indicação de que outra memória está danificada.") && GetLBWDataTableRowsCounterWorkAroundLimitCounter <= GetLBWDataTableRowsCounterWorkAroundInteractionsLimit)
                {
                    if (LightBaseConnectedDataReaderHolder != null)
                    {
                        if (!LightBaseConnectedDataReaderHolder.IsClosed)
                        {
                            LightBaseConnectedDataReaderHolder.Close();
                        }
                    }
                    GetLBWDataTableRowsCounterWorkAroundLimitCounter++;
                    blockRowsCountSet = true;
                    GetLBWDataTableRowsCounterWorkAround(multiValuedGroupName, confirmationQuery, ref rowsCount);
                }
            }

            //Note: Essa guambiarra serve para evitar o efeito colateral da chama recursiva! By Questor
            if (!blockRowsCountSet)
            {
                GetLBWDataTableRowsCounterWorkAroundLimitCounter = 0;
                rowsCount = GetLBWDataTableRowsCounterWorkAroundOutPut;
            }

            if (LightBaseConnectedDataReaderHolder != null)
            {
                if (!LightBaseConnectedDataReaderHolder.IsClosed)
                {
                    LightBaseConnectedDataReaderHolder.Close();
                }
            }

        }

        #region Note: "insert into" business! By Questor

        /// <summary>
        /// Guarda a query a ser inserida no próximo processo de inserção! By Questor
        /// </summary>
        private StringBuilder _InsertIntoStringBuilder = null;

        private StringBuilder InsertIntoStringBuilder
        {
            get
            {
                if (_InsertIntoStringBuilder == null)
                {
                    _InsertIntoStringBuilder = new StringBuilder();
                }
                return _InsertIntoStringBuilder;
            }
        }

        /// <summary>
        /// Guarda a lista de campos para o próximo processo de inserção! By Questor
        /// </summary>
        private List<FieldsNValues> _InsertIntoFieldsNValues = null;

        public List<FieldsNValues> InsertIntoFieldsNValues
        {
            get
            {
                if (_InsertIntoFieldsNValues == null)
                {
                    _InsertIntoFieldsNValues = new List<FieldsNValues>();
                }
                return _InsertIntoFieldsNValues;
            }
        }

        /// <summary>
        /// Guarda o campo parent para o próximo processo de inserção! By Questor
        /// </summary>
        private FieldsNValues _InsertIntoFieldNValueParent = null;

        public FieldsNValues InsertIntoFieldNValueParent
        {
            get
            {
                return _InsertIntoFieldNValueParent;
            }
        }

        /// <summary>
        /// Adiciona um campo multivalorado para o próximo processo de inserção! By Questor
        /// </summary>
        /// <param name="multiValuedGroup"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void AddField_InsertInto(string multiValuedGroup, string field, object value)
        {
            InsertIntoFieldsNValues.Add(new FieldsNValues(multiValuedGroup, null, field, value));
        }

        /// <summary>
        /// Adiciona um campo monovalorado para o próximo processo de inserção! By Questor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void AddField_InsertInto(string field, object value)
        {
            InsertIntoFieldsNValues.Add(new FieldsNValues(null, null, field, value));
        }

        /// <summary>
        /// Adiciona um parent para a inserção de campos multivalorados para o próximo processo de inserção! By Questor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void AddFieldParent_InsertInto(string field, object value)
        {
            _InsertIntoFieldNValueParent = new FieldsNValues(null, null, field, value);
        }

        /// <summary>
        /// Executa o última inserção setada! By Questor
        /// </summary>
        /// <returns></returns>
        public int Submit_InsertInto()
        {
            string fields0 = "";
            string fields1 = "";
            string separator = "";
            var u = InsertIntoFieldsNValues.Count;
            foreach (FieldsNValues InsertIntoField0 in InsertIntoFieldsNValues)
            {
                if (--u > 0)
                {
                    separator = ", ";
                }
                else
                {
                    separator = "";
                }

                if (!string.IsNullOrEmpty(InsertIntoField0.multiValuedGroup))
                {
                    fields0 = fields0 + InsertIntoField0.multiValuedGroup + "." + InsertIntoField0.field + separator;
                    fields1 = fields1 + "@" + InsertIntoField0.multiValuedGroup + "_" + InsertIntoField0.field + separator;
                }
                else
                {
                    fields0 = fields0 + InsertIntoField0.field + separator;
                    fields1 = fields1 + "@" + InsertIntoField0.field + separator;
                }
            }
            InsertIntoStringBuilder.Append(fields0);
            InsertIntoStringBuilder.Append(") values (");
            InsertIntoStringBuilder.Append(fields1);
            if (InsertIntoFieldNValueParent != null)
            {
                InsertIntoFieldsNValues.Add(InsertIntoFieldNValueParent);
                InsertIntoStringBuilder.Append(") parent " + InsertIntoFieldNValueParent.field + "=@Parent_" + InsertIntoFieldNValueParent.field);
            }
            else
            {
                InsertIntoStringBuilder.Append(")");
            }
            SetLBWCommand(InsertIntoStringBuilder.ToString());
            var lastItemCount = InsertIntoFieldsNValues.Count;
            foreach (FieldsNValues InsertIntoField1 in InsertIntoFieldsNValues)
            {
                if (--lastItemCount == 0 && InsertIntoFieldNValueParent != null)
                {
                    AddLightBaseParameter("Parent_" + InsertIntoField1.field, InsertIntoField1.value);
                }
                else if (!string.IsNullOrEmpty(InsertIntoField1.multiValuedGroup))
                {
                    AddLightBaseParameter(InsertIntoField1.multiValuedGroup + "_" + InsertIntoField1.field, InsertIntoField1.value);
                }
                else
                {
                    AddLightBaseParameter(InsertIntoField1.field, InsertIntoField1.value);
                }
            }
            return ExecuteNonQueryLBWCommand();
        }

        /// <summary>
        /// Seta um processo de inserção de forma segura! By Questor
        /// </summary>
        /// <param name="dataBase"></param>
        public void Set_InsertInto(string dataBase)
        {
            _InsertIntoFieldsNValues = null;
            _InsertIntoStringBuilder = null;
            _InsertIntoFieldNValueParent = null;
            InsertIntoStringBuilder.Append("insert into " + dataBase + " (");
        }

        /// <summary>
        /// Tipo "campo" usado no processo de update! By Questor
        /// </summary>
        internal class FieldsNValues
        {

            private string _multiValuedGroup = "";

            public string multiValuedGroup
            {
                get { return _multiValuedGroup; }
                set { _multiValuedGroup = value; }
            }

            private int? _row = null;

            public int? row
            {
                get
                {
                    //if(Submit_Update_UpdateWhenProblematicRowPosition > 0)
                    //{
                    //    return Submit_Update_UpdateWhenProblematicRowPosition;
                    //}
                    return _row;
                }
                set { _row = value; }
            }

            private string _field = "";

            public string field
            {
                get { return _field; }
                set { _field = value; }
            }

            private object _value = "";

            public object value
            {
                get { return _value; }
                set { _value = value; }
            }

            public FieldsNValues(string multiValuedGroup_, int? row_, string field_, object value_)
            {
                multiValuedGroup = multiValuedGroup_;
                row = row_;
                field = field_;
                //if (value_ is string)
                //{
                //    value = LBWEncoding(value_.ToString());
                //}
                //else
                //{
                value = value_;
                //}

            }

        }

        #endregion

        #region Note: "update" business! By Questor

        /// <summary>
        /// Guarda a string a ser usada na execução do próximo processo de update! By Questor
        /// </summary>
        private StringBuilder _UpdateStringBuilder = null;

        private StringBuilder UpdateStringBuilder
        {
            get
            {
                if (_UpdateStringBuilder == null)
                {
                    _UpdateStringBuilder = new StringBuilder();
                }
                return _UpdateStringBuilder;
            }
        }

        //private StringBuilder _UpdateStringBuilderWhenProblematic = null;

        //private StringBuilder UpdateStringBuilderWhenProblematic
        //{
        //    get
        //    {
        //        if (_UpdateStringBuilderWhenProblematic == null)
        //        {
        //            _UpdateStringBuilderWhenProblematic = new StringBuilder();
        //        }
        //        return _UpdateStringBuilderWhenProblematic;
        //    }
        //}

        //private string _dataBaseOnUpdate = null;

        //private string dataBaseOnUpdate
        //{
        //    get
        //    {
        //        return _dataBaseOnUpdate;
        //    }
        //}


        /// <summary>
        /// Guarda a lista de campos adicionados para o próximo processo de update! By Questor
        /// </summary>
        private List<FieldsNValues> _UpdateFieldsNValues = null;

        public List<FieldsNValues> UpdateFieldsNValues
        {
            get
            {
                if (_UpdateFieldsNValues == null)
                {
                    _UpdateFieldsNValues = new List<FieldsNValues>();
                }
                return _UpdateFieldsNValues;
            }
        }

        /// <summary>
        /// Guarda o where para o próximo processo de update! By Questor
        /// </summary>
        private FieldsNValues _UpdateFieldNValueWhere = null;

        public FieldsNValues UpdateFieldNValueWhere
        {
            get
            {
                return _UpdateFieldNValueWhere;
            }
        }

        /// <summary>
        /// Adiciona um campo monovalorado para o próximo processo de update! By Questor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void AddField_Update(string field, object value)
        {
            UpdateFieldsNValues.Add(new FieldsNValues(null, null, field, value));
        }

        /// <summary>
        /// Adiciona um campo multivalorado para o próximo processo de update! By Questor
        /// </summary>
        /// <param name="multiValuedGroup"></param>
        /// <param name="row"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void AddField_Update(string multiValuedGroup, int row, string field, object value)
        {
            UpdateFieldsNValues.Add(new FieldsNValues(multiValuedGroup, row, field, value));
        }

        /// <summary>
        /// Adiciona o where para o próximo processo de update! By Questor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void AddWhere_Update(string field, object value)
        {
            _UpdateFieldNValueWhere = new FieldsNValues(null, null, field, value);
        }

        //private static int Submit_Update_UpdateWhenProblematicRowPosition = 0;
        //public int Submit_Update_UpdateWhenProblematic(int rowPosition)
        //{
        //    int Submit_Update_UpdateWhenProblematicOutPut = -1;
        //    if (rowPosition > 0)
        //    {
        //        SetLBWCommand("delete from " + dataBaseOnUpdate + "." + UpdateFieldsNValues[0].multiValuedGroup + "[" +
        //                      UpdateFieldsNValues[0].row + "] where " + UpdateFieldNValueWhere.field + "=@Where_" +
        //                      UpdateFieldNValueWhere.field);
        //        AddLightBaseParameter("Where_" + UpdateFieldNValueWhere.field, UpdateFieldNValueWhere.value);
        //        Submit_Update_UpdateWhenProblematicOutPut = ExecuteNonQueryLBWCommand();
        //        Submit_Update_UpdateWhenProblematicRowPosition = rowPosition;
        //        if (Submit_Update_UpdateWhenProblematicOutPut > 0)
        //        {
        //            return Submit_Update();
        //        }
        //    }
        //    else
        //    {
        //        return Submit_Update();
        //    }
        //    return Submit_Update_UpdateWhenProblematicOutPut;
        //}

        /// <summary>
        /// Executa o último update setado! By Questor
        /// </summary>
        /// <returns></returns>
        public int Submit_Update()
        {

            string fields0 = "";
            string separator = "";
            var u = UpdateFieldsNValues.Count;
            foreach (FieldsNValues UpdateField0 in UpdateFieldsNValues)
            {
                if (--u > 0)
                {
                    separator = ", ";
                }
                else
                {
                    separator = "";
                }

                if (!string.IsNullOrEmpty(UpdateField0.multiValuedGroup))
                {
                    fields0 = fields0 + UpdateField0.multiValuedGroup + "[" + UpdateField0.row + "]." + UpdateField0.field + "=@" + UpdateField0.multiValuedGroup + "_" + UpdateField0.field + separator;
                }
                else
                {
                    //update discos set Titulo = "Nulo" where Titulo = "Erotica 2"
                    fields0 = fields0 + UpdateField0.field + "=@" + UpdateField0.field + separator;
                }
            }
            UpdateStringBuilder.Append(fields0);
            UpdateFieldsNValues.Add(UpdateFieldNValueWhere);
            UpdateStringBuilder.Append(" where " + UpdateFieldNValueWhere.field + "=@Where_" + UpdateFieldNValueWhere.field);

            SetLBWCommand(UpdateStringBuilder.ToString());
            var lastItemCount = UpdateFieldsNValues.Count;
            foreach (FieldsNValues UpdateField1 in UpdateFieldsNValues)
            {
                if (--lastItemCount == 0)
                {
                    AddLightBaseParameter("Where_" + UpdateField1.field, UpdateField1.value);
                }
                else if (!string.IsNullOrEmpty(UpdateField1.multiValuedGroup))
                {
                    AddLightBaseParameter(UpdateField1.multiValuedGroup + "_" + UpdateField1.field, UpdateField1.value);
                }
                else
                {
                    AddLightBaseParameter(UpdateField1.field, UpdateField1.value);
                }
            }
            return ExecuteNonQueryLBWCommand();
        }

        /// <summary>
        /// Seta um processo de inserção de forma segura! By Questor
        /// </summary>
        /// <param name="dataBase"></param>
        public void Set_Update(string dataBase)
        {
            _UpdateFieldsNValues = null;
            _UpdateStringBuilder = null;
            //Submit_Update_UpdateWhenProblematicRowPosition = 0;
            //_dataBaseOnUpdate = dataBase;
            //UpdateStringBuilder.Append("update " + dataBaseOnUpdate + " set ");
            UpdateStringBuilder.Append("update " + dataBase + " set ");
        }

        #endregion


        /// <summary>
        /// Converte valores bool de/para o LightBase de forma segura! By Questor
        /// </summary>
        /// <param name="valueToConvert"></param>
        /// <returns></returns>
        public static bool ConvertToBooleanGuaranteedNSafe(object valueToConvert)
        {
            if (valueToConvert != null && (string)valueToConvert != "")
            {
                return Convert.ToBoolean(valueToConvert);
            }
            return false;
        }


        /// <summary>
        /// Converte valores DateTime de/para o LightBase de forma segura! By Questor
        /// </summary>
        /// <param name="objToDateTime"></param>
        /// <returns></returns>
        static public DateTime? ConvertToDateTime_Safe(object objToDateTime)
        {
            if (objToDateTime != null && objToDateTime.ToString() != "")
            {
                return Convert.ToDateTime(objToDateTime);
            }
            return null;
        }


        /// <summary>
        /// Converte valores DateTime (em string, apenas a Data) de/para o LightBase de forma segura! By Questor
        /// </summary>
        /// <param name="dateToAdjust"></param>
        /// <returns></returns>
        static public string AdjustDate_ptBr(string dateToAdjust)
        {
            IFormatProvider culture = new System.Globalization.CultureInfo("pt-BR", true);
            DateTime dateTimeHolder = DateTime.Parse(dateToAdjust, culture);
            return dateTimeHolder.ToString("dd/MM/yyyy");
        }


        /// <summary>
        /// Converte valores DateTime (em string, apenas a Time) de/para o LightBase de forma segura! By Questor
        /// </summary>
        /// <param name="timeToAdjust"></param>
        /// <returns></returns>
        static public string AdjustTime_ptBr(string timeToAdjust)
        {
            IFormatProvider culture = new System.Globalization.CultureInfo("pt-BR", true);
            DateTime DateTimeHolder = DateTime.Parse(timeToAdjust, culture);
            return DateTime.Now.ToString("HH:mm:ss");
        }

    }
}
