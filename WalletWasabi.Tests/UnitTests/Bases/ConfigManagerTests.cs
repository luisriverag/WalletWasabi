using NBitcoin;
using System.IO;
using System.Threading.Tasks;
using WalletWasabi.Bases;
using WalletWasabi.Tests.Helpers;
using WalletWasabi.WabiSabi.Backend;
using WalletWasabi.WabiSabi.Models;
using Xunit;
using Key = Avalonia.Remote.Protocol.Input.Key;

namespace WalletWasabi.Tests.UnitTests.Bases;

/// <summary>
/// Tests for <see cref="ConfigManager"/>
/// </summary>
public class ConfigManagerTests
{
	/// <summary>
	/// Tests <see cref="ConfigManager.CheckFileChange{T}(string, T)"/>.
	/// </summary>
	[Fact]
	public async Task CheckFileChangeTestAsync()
	{
		string workDirectory = await Common.GetEmptyWorkDirAsync();
		string configPath = Path.Combine(workDirectory, $"{nameof(CheckFileChangeTestAsync)}.json");

		// Create config and store it.
		WabiSabiConfig config = new ();
		config.AnnouncerConfig = config.AnnouncerConfig with
		{
			Key = "nsec13zprl2n0acj9cgrteyfgwcuhrk3xgac775zqyflhxxsfvwjvfe9qrg9l0v"
		};
		config.SetFilePath(configPath);
		config.ToFile();

		// Check that the stored config corresponds to the expected "vanilla" config.
		{
			string expectedFileContents = GetVanillaConfigString();
			string actualFileContents = ReadAllTextAndNormalize(configPath);

			Assert.Equal(expectedFileContents, actualFileContents);

			// No change was done.
			Assert.False(ConfigManager.CheckFileChange(configPath, config));
		}

		static string GetVanillaConfigString()
				=> $$"""
			{
			  "Network": "Main",
			  "BitcoinCoreRpcEndPoint": "127.0.0.1:8332",
			  "BitcoinRpcConnectionString": "user:password",
			  "ConfirmationTarget": 108,
			  "DoSSeverity": "0.10",
			  "DoSMinTimeForFailedToVerify": "31d 0h 0m 0s",
			  "DoSMinTimeForCheating": "1d 0h 0m 0s",
			  "DoSPenaltyFactorForDisruptingConfirmation": 0.2,
			  "DoSPenaltyFactorForDisruptingSignalReadyToSign": 1.0,
			  "DoSPenaltyFactorForDisruptingSigning": 1.0,
			  "DoSPenaltyFactorForDisruptingByDoubleSpending": 3.0,
			  "DoSMinTimeInPrison": "0d 0h 20m 0s",
			  "MinRegistrableAmount": "0.00005",
			  "MaxRegistrableAmount": "43000.00",
			  "AllowNotedInputRegistration": true,
			  "StandardInputRegistrationTimeout": "0d 1h 0m 0s",
			  "BlameInputRegistrationTimeout": "0d 0h 3m 0s",
			  "ConnectionConfirmationTimeout": "0d 0h 1m 0s",
			  "OutputRegistrationTimeout": "0d 0h 1m 0s",
			  "TransactionSigningTimeout": "0d 0h 1m 0s",
			  "FailFastOutputRegistrationTimeout": "0d 0h 3m 0s",
			  "FailFastTransactionSigningTimeout": "0d 0h 1m 0s",
			  "RoundExpiryTimeout": "0d 0h 5m 0s",
			  "MaxInputCountByRound": 100,
			  "MinInputCountByRoundMultiplier": 0.5,
			  "MinInputCountByBlameRoundMultiplier": 0.4,
			  "RoundDestroyerThreshold": 375,
			  "CoordinatorExtPubKey": "xpub6C13JhXzjAhVRgeTcRSWqKEPe1vHi3Tmh2K9PN1cZaZFVjjSaj76y5NNyqYjc2bugj64LVDFYu8NZWtJsXNYKFb9J94nehLAPAKqKiXcebC",
			  "CoordinatorExtPubKeyCurrentDepth": 1,
			  "MaxSuggestedAmountBase": "0.10",
			  "RoundParallelization": 1,
			  "WW200CompatibleLoadBalancing": false,
			  "WW200CompatibleLoadBalancingInputSplit": 0.75,
			  "CoordinatorIdentifier": "CoinJoinCoordinatorIdentifier",
			  "AllowP2wpkhInputs": true,
			  "AllowP2trInputs": true,
			  "AllowP2wpkhOutputs": true,
			  "AllowP2trOutputs": true,
			  "AllowP2pkhOutputs": false,
			  "AllowP2shOutputs": false,
			  "AllowP2wshOutputs": false,
			  "DelayTransactionSigning": false,
			  "AnnouncerConfig": {
			    "CoordinatorName": "Coordinator",
			    "IsEnabled": false,
			    "CoordinatorDescription": "WabiSabi Coinjoin Coordinator",
			    "CoordinatorUri": "https://api.example.com/",
			    "AbsoluteMinInputCount": 21,
			    "ReadMoreUri": "https://api.example.com/",
			    "RelayUris": [
			      "wss://relay.primal.net"
			    ],
			    "Key": "nsec13zprl2n0acj9cgrteyfgwcuhrk3xgac775zqyflhxxsfvwjvfe9qrg9l0v"
			  }
			}
			""".ReplaceLineEndings("\n");
	}

	private static string ReadAllTextAndNormalize(string configPath)
		=> File.ReadAllText(configPath).ReplaceLineEndings("\n");
}
