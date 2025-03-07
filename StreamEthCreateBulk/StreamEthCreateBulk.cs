/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

01/06/2023	1.0.0.1		SVD, Skyline	Initial version
****************************************************************************
*/

namespace StreamEthCreateBulk
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.ConnectorAPI.BridgeTechnologies.VBProbeSeries;
	using Skyline.DataMiner.ConnectorAPI.BridgeTechnologies.VBProbeSeries.Ethernet;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
	using Skyline.DataMiner.Core.InterAppCalls.Common.Shared;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			// Get user input
			////string elementName = "BT VB Probe Series";
			string elementName = engine.GetScriptParam("Element Name").Value;

			// Find Element
			var element = engine.FindElement(elementName);
			if (element == null)
			{
				engine.GenerateInformation($"Could not find element with name '{elementName}'.");
				return;
			}
			else
			{
				engine.GenerateInformation($"Found element with name '{elementName}' - elemendID '{element.DmaId}/{element.ElementId}'");
			}

			// Build InterApp Messages
			var command = InterAppCallFactory.CreateNew();
			command.Source = new Source("BT VB Series - InterAppDemo - Streams - Eth - Create Bulk");
			command.ReturnAddress = new ReturnAddress(element.DmaId, element.ElementId, 9000001);

			for (int i = 1; i <= 10; i++)
			{
				var message = new CreateEthStream
				{
					StreamData = MakeStreamData(i),
					Source = new Source("BT VB Series - InterAppDemo - Streams - Eth - Create Bulk"),
				};

				command.Messages.Add(message);
			}

			// Process InterApp Messages
			foreach (var responseMessage in command.Send(Engine.SLNetRaw, element.DmaId, element.ElementId, 9_000_000, new TimeSpan(0, 0, 10), InterApp.KnownTypes))
			{
				if (responseMessage != null)
				{
					if (responseMessage is CreateEthStreamResult result)
					{
						engine.GenerateInformation(result.Description);
					}
					else
					{
						engine.GenerateInformation($"{nameof(responseMessage)} is not of expected type '{nameof(CreateEthStreamResult)}'.{Environment.NewLine}{responseMessage}");
					}
				}
				else
				{
					engine.GenerateInformation($"{nameof(responseMessage)} is null.");
				}
			}
		}

		public EthStreamData MakeStreamData(int i)
		{
			var streamData = new EthStreamData
			{
				Name = $"Demo_CreateBulk_{i}",
				Address = $"239.0.20.1{i}",
				Port = $"591{i}",
				VLanId = "0",

				EthThresholds = "Default",
				EtrThresholds = "Default",
				ServiceThresholds = "Default",
				PidThresholds = "Default",
				ReferenceThresholds = "Default",
				VbcThresholds = "Default",
				ContentThresholds = "Default",

				Join = true,
				JoinInterface = "eth0",

				IsEtrEnabled = false,
				EtrEngine = "1",

				IsFecEnabled = false,
				T2miStream = String.Empty,
				T2miPid = "0",
				T2miPlpId = "0",
				Dash7Stream1 = String.Empty,
				Dash7Stream2 = String.Empty,

				SsmAddress = "0.0.0.1",
				SsmName = String.Empty,
				SsmAddress2 = "0.0.0.0",
				SsmName2 = String.Empty,
				SsmAddress3 = "0.0.0.0",
				SsmName3 = String.Empty,
				SsmAddress4 = "0.0.0.0",
				SsmName4 = String.Empty,
				SsmAddress5 = "0.0.0.0",
				SsmName5 = String.Empty,

				IsSrtEnabled = true,
				SrtHost = String.Empty,
				SrtPort = 1,
				SrtMode = SrtMode.Listener,
				SrtPassphrase = String.Empty,
				SrtLatency = 0,

				Page = 1,
			};

			return streamData;
		}
	}
}