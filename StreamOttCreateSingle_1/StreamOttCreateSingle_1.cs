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

namespace OttStreamCreateSingle
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.ConnectorAPI.BridgeTechnologies.VBProbeSeries;
	using Skyline.DataMiner.ConnectorAPI.BridgeTechnologies.VBProbeSeries.OTT;
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

			// Build InterApp Message
			var command = InterAppCallFactory.CreateNew();
			command.Source = new Source("BT VB Series - InterAppDemo - Streams - OTT - Create Single");
			command.ReturnAddress = new ReturnAddress(element.DmaId, element.ElementId, 9000001);

			var message = new CreateOttChannel
			{
				ChannelData = MakeChannelData(),
				Source = new Source("BT VB Series - InterAppDemo - Streams - OTT - Create Single"),
			};

			command.Messages.Add(message);

			// Process InterApp Message
			foreach (var responseMessage in command.Send(Engine.SLNetRaw, element.DmaId, element.ElementId, 9000000, new TimeSpan(0, 0, 10), InterApp.KnownTypes))
			{
				if (responseMessage != null)
				{
					if (responseMessage is CreateOttChannelResult result)
					{
						engine.GenerateInformation(result.Description);
					}
					else
					{
						engine.GenerateInformation($"{nameof(responseMessage)} is not of expected type '{nameof(CreateOttChannelResult)}'.{Environment.NewLine}{responseMessage}");
					}
				}
				else
				{
					engine.GenerateInformation($"{nameof(responseMessage)} is null.");
				}
			}
		}

		public OttChannelData MakeChannelData()
		{
			var streamData = new OttChannelData
			{
				// General
				Name = "Demo_CreateSingle_1",
				Address = "239.0.20.1",

				Enable = false,
				Engine = 0,
				Page = 0,

				Threshold = "Default",
				VbcThreshold = "Default",
				ContentThreshold = null,
				MeasurementMode = 0,

				PlayerUrl = null,

				LiveTargetSegment = 0,

				Rtmp = false,
				RtmpLive = false,
				NoMasterMonitor = false,
				ExtractThumbnails = false,
				Alignment = false,

				// DRM
				DrmSystem = 0,
				DrmHostname = null,
				DrmUsername = null,
				DrmPassword = null,
				AccountId = null,
				ContentId = null,
				CryptoPeriod = 0,
				FixedKey = null,
				FixedIv = null,

				// Advanced manifest
				AdvancedManifest = false,
				Method = null,
				ContentType = null,
			};

			return streamData;
		}
	}
}