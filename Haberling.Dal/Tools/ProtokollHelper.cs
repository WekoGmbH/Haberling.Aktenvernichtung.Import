using Haberling.ImportDto;
using Sagede.Core.Tools;
using Sagede.OfficeLine.Data;
using Sagede.OfficeLine.Data.Entities;
using Sagede.OfficeLine.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.Dal
{
	public static class ProtokollHelper
	{

		public static void AppendProtokollItem(
			Mandant mandant,
			ProtokollLogType logType,
			int belID,
			string belegart,
			short belegjahr,
			string belegnummer,
			string kto,
			string matchcode,
			DateTime? belegdatum,
			string wkz,
			int fehlerID,
			string fehlertext,
			bool statusKz,
			string hinweis
			)
		{

			var entitiesApdater = mandant.MainDevice.Entities.Adapter as EntityDeviceAdapter;
			var protokollTabelInfos = entitiesApdater.GetTableInfo(typeof(Sagede.OfficeLine.Data.Entities.Main.TempProtokollItem));

			var protokollItem = mandant.MainDevice.Entities.TempProtokoll.CreateItem();
			protokollItem.BelegID = belID;
			protokollItem.Belegart = belegart.Left(50);
			protokollItem.Belegjahr = belegjahr;
			protokollItem.Belegnummer = belegnummer.Left(50);
			protokollItem.Wkz = wkz.Left(3);
			protokollItem.Belegdatum = belegdatum;
			protokollItem.Matchcode = matchcode.Left(protokollTabelInfos.Columns["Matchcode"].MaxLength);
			protokollItem.Kto = kto;
			protokollItem.ProtokollTyp = (short)logType;
			protokollItem.FehlerID = fehlerID;
			protokollItem.FehlerText = fehlertext;
			protokollItem.StatusKz = statusKz;
			protokollItem.Datum1 = DateTime.Now;
			protokollItem.Text8 = hinweis.Left(250);
			protokollItem.ConnID = mandant.MultiUserServiceMain.ConnectionId;
			protokollItem.Save();

		}

		public static Sagede.OfficeLine.Data.Entities.Main.TempProtokollSet GetTempProtokollItems(Mandant mandant, int connID, ProtokollLogType logType)
		{
			var parameterList = new QueryParameterList();
			parameterList.AddClauseParameter("ConnId", connID);
			parameterList.AddClauseParameter("ProtokollTyp", (short)logType);
			//parameterList.AddClauseParameter(new ClauseParameter() { ComparisonType = ClauseParameterComparisonType.Equals, FieldName = "ProtokollTyp", Value = (short)logType });

			return mandant.MainDevice.Entities.TempProtokoll.GetList(parameterList);
		}

		public static void ClearProtokollItems(Mandant mandant, int connID, ProtokollLogType logType)
		{
			try
			{
				var parameterList = new QueryParameterList();
				parameterList.AddClauseParameter("ConnId", connID);
				parameterList.AddClauseParameter("ProtokollTyp", (short)logType);
				//parameterList.AddClauseParameter(new ClauseParameter() { ComparisonType = ClauseParameterComparisonType.Equals, FieldName = "ProtokollTyp", Value = (short)logType });

				var protokollItemSet = mandant.MainDevice.Entities.TempProtokoll.GetList(parameterList);
				mandant.MainDevice.GenericConnection.BeginTransaction();
				protokollItemSet.ToList().ForEach(x => x.Delete());
				mandant.MainDevice.GenericConnection.CommitTransaction();
			}
			catch (Exception ex)
			{
				mandant.MainDevice.GenericConnection.RollbackTransaction();
				TraceLog.LogException(ex);
				throw;
			}

		}

	}
}
