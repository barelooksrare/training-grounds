using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Solana.Unity;
using Solana.Unity.Programs.Abstract;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Core.Sockets;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using TrainingGrounds;
using TrainingGrounds.Program;
using TrainingGrounds.Errors;
using TrainingGrounds.Accounts;
using TrainingGrounds.Types;

namespace TrainingGrounds
{
    namespace Accounts
    {
        public partial class GameAdminProof
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 1342582370109081945UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{89, 201, 13, 100, 141, 207, 161, 18};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "G22mVbZMiTB";
            public PublicKey Admin { get; set; }

            public CollectionIdentifier Collection { get; set; }

            public static GameAdminProof Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                GameAdminProof result = new GameAdminProof();
                result.Admin = _data.GetPubKey(offset);
                offset += 32;
                offset += CollectionIdentifier.Deserialize(_data, offset, out var resultCollection);
                result.Collection = resultCollection;
                return result;
            }
        }

        public partial class ProgramAdminProof
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 6828378895819667010UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{66, 246, 26, 109, 109, 70, 195, 94};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "CCcKHfdkFxy";
            public PublicKey Admin { get; set; }

            public static ProgramAdminProof Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                ProgramAdminProof result = new ProgramAdminProof();
                result.Admin = _data.GetPubKey(offset);
                offset += 32;
                return result;
            }
        }

        public partial class Club
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 13069064119827879508UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{84, 214, 36, 249, 146, 164, 94, 181};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "FC2BQypmzAQ";
            public byte Version { get; set; }

            public PublicKey ClubAdmin { get; set; }

            public CollectionIdentifier CollectionIdentifier { get; set; }

            public PublicKey MetadataMint { get; set; }

            public PublicKey RewardMint { get; set; }

            public byte RewardMintDecimals { get; set; }

            public bool IsActive { get; set; }

            public uint MemberCount { get; set; }

            public uint ActiveGames { get; set; }

            public byte[] Reserved { get; set; }

            public GameParams GameParams { get; set; }

            public static Club Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Club result = new Club();
                result.Version = _data.GetU8(offset);
                offset += 1;
                result.ClubAdmin = _data.GetPubKey(offset);
                offset += 32;
                offset += CollectionIdentifier.Deserialize(_data, offset, out var resultCollectionIdentifier);
                result.CollectionIdentifier = resultCollectionIdentifier;
                if (_data.GetBool(offset++))
                {
                    result.MetadataMint = _data.GetPubKey(offset);
                    offset += 32;
                }

                result.RewardMint = _data.GetPubKey(offset);
                offset += 32;
                result.RewardMintDecimals = _data.GetU8(offset);
                offset += 1;
                result.IsActive = _data.GetBool(offset);
                offset += 1;
                result.MemberCount = _data.GetU32(offset);
                offset += 4;
                result.ActiveGames = _data.GetU32(offset);
                offset += 4;
                result.Reserved = _data.GetBytes(offset, 64);
                offset += 64;
                offset += GameParams.Deserialize(_data, offset, out var resultGameParams);
                result.GameParams = resultGameParams;
                return result;
            }
        }

        public partial class Game
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 1331205435963103771UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{27, 90, 166, 125, 74, 100, 121, 18};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "5aNQXizG8jB";
            public PublicKey Club { get; set; }

            public PublicKey Player { get; set; }

            public long StartTime { get; set; }

            public static Game Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Game result = new Game();
                result.Club = _data.GetPubKey(offset);
                offset += 32;
                result.Player = _data.GetPubKey(offset);
                offset += 32;
                result.StartTime = _data.GetS64(offset);
                offset += 8;
                return result;
            }
        }

        public partial class Player
        {
            public static ulong ACCOUNT_DISCRIMINATOR => 15766710478567431885UL;
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{205, 222, 112, 7, 165, 155, 206, 218};
            public static string ACCOUNT_DISCRIMINATOR_B58 => "bSBoKNsSHuj";
            public PublicKey Mint { get; set; }

            public PublicKey Club { get; set; }

            public byte Energy { get; set; }

            public long RechargeStartTime { get; set; }

            public uint GamesPlayed { get; set; }

            public static Player Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                Player result = new Player();
                result.Mint = _data.GetPubKey(offset);
                offset += 32;
                result.Club = _data.GetPubKey(offset);
                offset += 32;
                result.Energy = _data.GetU8(offset);
                offset += 1;
                result.RechargeStartTime = _data.GetS64(offset);
                offset += 8;
                result.GamesPlayed = _data.GetU32(offset);
                offset += 4;
                return result;
            }
        }
    }

    namespace Errors
    {
        public enum TrainingGroundsErrorKind : uint
        {
            ProgramAuthorityMismatch = 6000U,
            CollectionProofInvalid = 6001U,
            CollectionKeyMismatch = 6002U,
            TokenOwnerMismatch = 6003U,
            OwnerBalanceMismatch = 6004U,
            MintNotNft = 6005U,
            OutOfEnergy = 6006U,
            EnergyCalculationFailed = 6007U,
            ClubInactive = 6008U,
            InvalidInput = 6009U
        }
    }

    namespace Types
    {
        public partial class GameParams
        {
            public ulong MaxRewardsPerGame { get; set; }

            public byte MaxPlayerEnergy { get; set; }

            public long EnergyRechargeMinutes { get; set; }

            public bool BurnRemainingTokens { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU64(MaxRewardsPerGame, offset);
                offset += 8;
                _data.WriteU8(MaxPlayerEnergy, offset);
                offset += 1;
                _data.WriteS64(EnergyRechargeMinutes, offset);
                offset += 8;
                _data.WriteBool(BurnRemainingTokens, offset);
                offset += 1;
                return offset - initialOffset;
            }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out GameParams result)
            {
                int offset = initialOffset;
                result = new GameParams();
                result.MaxRewardsPerGame = _data.GetU64(offset);
                offset += 8;
                result.MaxPlayerEnergy = _data.GetU8(offset);
                offset += 1;
                result.EnergyRechargeMinutes = _data.GetS64(offset);
                offset += 8;
                result.BurnRemainingTokens = _data.GetBool(offset);
                offset += 1;
                return offset - initialOffset;
            }
        }

        public enum CollectionIdentifierType : byte
        {
            Collection,
            Creator
        }

        public partial class CollectionType
        {
            public PublicKey Pubkey { get; set; }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out CollectionType result)
            {
                int offset = initialOffset;
                result = new CollectionType();
                result.Pubkey = _data.GetPubKey(offset);
                offset += 32;
                return offset - initialOffset;
            }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WritePubKey(Pubkey, offset);
                offset += 32;
                return offset - initialOffset;
            }
        }

        public partial class CreatorType
        {
            public PublicKey Pubkey { get; set; }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out CreatorType result)
            {
                int offset = initialOffset;
                result = new CreatorType();
                result.Pubkey = _data.GetPubKey(offset);
                offset += 32;
                return offset - initialOffset;
            }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WritePubKey(Pubkey, offset);
                offset += 32;
                return offset - initialOffset;
            }
        }

        public partial class CollectionIdentifier
        {
            public CollectionType CollectionValue { get; set; }

            public CreatorType CreatorValue { get; set; }

            public int Serialize(byte[] _data, int initialOffset)
            {
                int offset = initialOffset;
                _data.WriteU8((byte)Type, offset);
                offset += 1;
                switch (Type)
                {
                    case CollectionIdentifierType.Collection:
                        offset += CollectionValue.Serialize(_data, offset);
                        break;
                    case CollectionIdentifierType.Creator:
                        offset += CreatorValue.Serialize(_data, offset);
                        break;
                }

                return offset - initialOffset;
            }

            public CollectionIdentifierType Type { get; set; }

            public static int Deserialize(ReadOnlySpan<byte> _data, int initialOffset, out CollectionIdentifier result)
            {
                int offset = initialOffset;
                result = new CollectionIdentifier();
                result.Type = (CollectionIdentifierType)_data.GetU8(offset);
                offset += 1;
                switch (result.Type)
                {
                    case CollectionIdentifierType.Collection:
                    {
                        CollectionType tmpCollectionValue = new CollectionType();
                        offset += CollectionType.Deserialize(_data, offset, out tmpCollectionValue);
                        result.CollectionValue = tmpCollectionValue;
                        break;
                    }

                    case CollectionIdentifierType.Creator:
                    {
                        CreatorType tmpCreatorValue = new CreatorType();
                        offset += CreatorType.Deserialize(_data, offset, out tmpCreatorValue);
                        result.CreatorValue = tmpCreatorValue;
                        break;
                    }
                }

                return offset - initialOffset;
            }
        }
    }

    public partial class TrainingGroundsClient : TransactionalBaseClient<TrainingGroundsErrorKind>
    {
        public TrainingGroundsClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameAdminProof>>> GetGameAdminProofsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = GameAdminProof.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameAdminProof>>(res);
            List<GameAdminProof> resultingAccounts = new List<GameAdminProof>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => GameAdminProof.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<GameAdminProof>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ProgramAdminProof>>> GetProgramAdminProofsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = ProgramAdminProof.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ProgramAdminProof>>(res);
            List<ProgramAdminProof> resultingAccounts = new List<ProgramAdminProof>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => ProgramAdminProof.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<ProgramAdminProof>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Club>>> GetClubsAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = Club.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Club>>(res);
            List<Club> resultingAccounts = new List<Club>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Club.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Club>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Game>>> GetGamesAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = Game.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Game>>(res);
            List<Game> resultingAccounts = new List<Game>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Game.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Game>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Player>>> GetPlayersAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>{new Solana.Unity.Rpc.Models.MemCmp{Bytes = Player.ACCOUNT_DISCRIMINATOR_B58, Offset = 0}};
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Player>>(res);
            List<Player> resultingAccounts = new List<Player>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Player.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<Player>>(res, resultingAccounts);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<GameAdminProof>> GetGameAdminProofAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<GameAdminProof>(res);
            var resultingAccount = GameAdminProof.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<GameAdminProof>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<ProgramAdminProof>> GetProgramAdminProofAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<ProgramAdminProof>(res);
            var resultingAccount = ProgramAdminProof.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<ProgramAdminProof>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<Club>> GetClubAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<Club>(res);
            var resultingAccount = Club.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<Club>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<Game>> GetGameAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<Game>(res);
            var resultingAccount = Game.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<Game>(res, resultingAccount);
        }

        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<Player>> GetPlayerAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<Player>(res);
            var resultingAccount = Player.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<Player>(res, resultingAccount);
        }

        public async Task<SubscriptionState> SubscribeGameAdminProofAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, GameAdminProof> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                GameAdminProof parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = GameAdminProof.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeProgramAdminProofAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, ProgramAdminProof> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                ProgramAdminProof parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = ProgramAdminProof.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeClubAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, Club> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Club parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Club.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribeGameAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, Game> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Game parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Game.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<SubscriptionState> SubscribePlayerAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, Player> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                Player parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = Player.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        public async Task<RequestResult<string>> SendAddProgramAdminAsync(AddProgramAdminAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.AddProgramAdmin(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRemoveProgramAdminAsync(RemoveProgramAdminAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.RemoveProgramAdmin(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendRegisterClubAsync(RegisterClubAccounts accounts, CollectionIdentifier identifier, GameParams @params, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.RegisterClub(accounts, identifier, @params, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendEditGameAsync(EditGameAccounts accounts, GameParams @params, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.EditGame(accounts, @params, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendFundGameAsync(FundGameAccounts accounts, ulong amount, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.FundGame(accounts, amount, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendWithdrawFundsAsync(WithdrawFundsAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.WithdrawFunds(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendSetMetadataMintAsync(SetMetadataMintAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.SetMetadataMint(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendStartGameAsync(StartGameAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.StartGame(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        public async Task<RequestResult<string>> SendCompleteGameAsync(CompleteGameAccounts accounts, ulong tokens, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.TrainingGroundsProgram.CompleteGame(accounts, tokens, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        protected override Dictionary<uint, ProgramError<TrainingGroundsErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<TrainingGroundsErrorKind>>{{6000U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.ProgramAuthorityMismatch, "Signer does not match program authority")}, {6001U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.CollectionProofInvalid, "Collection Proof is invalid")}, {6002U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.CollectionKeyMismatch, "Collection Key Mismatch")}, {6003U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.TokenOwnerMismatch, "Caller does not own the token account")}, {6004U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.OwnerBalanceMismatch, "Caller does not own the NFT")}, {6005U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.MintNotNft, "Mint is not an NFT")}, {6006U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.OutOfEnergy, "Energy depleted")}, {6007U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.EnergyCalculationFailed, "Energy calculation failed")}, {6008U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.ClubInactive, "Club inactive")}, {6009U, new ProgramError<TrainingGroundsErrorKind>(TrainingGroundsErrorKind.InvalidInput, "Invalid input")}, };
        }
    }

    namespace Program
    {
        public class AddProgramAdminAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey ProgramAdmin { get; set; }

            public PublicKey ProgramAdminProof { get; set; }

            public PublicKey ProgramData { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class RemoveProgramAdminAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey ProgramAdmin { get; set; }

            public PublicKey ProgramAdminProof { get; set; }

            public PublicKey ProgramData { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class RegisterClubAccounts
        {
            public PublicKey ProgramAdmin { get; set; }

            public PublicKey ProgramAdminProof { get; set; }

            public PublicKey Club { get; set; }

            public PublicKey ClubAdmin { get; set; }

            public PublicKey RewardMint { get; set; }

            public PublicKey RewardAccount { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class EditGameAccounts
        {
            public PublicKey ClubAdmin { get; set; }

            public PublicKey Club { get; set; }
        }

        public class FundGameAccounts
        {
            public PublicKey ClubAdmin { get; set; }

            public PublicKey SourceTokenAccount { get; set; }

            public PublicKey Club { get; set; }

            public PublicKey RewardMint { get; set; }

            public PublicKey RewardAccount { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class WithdrawFundsAccounts
        {
            public PublicKey ClubAdmin { get; set; }

            public PublicKey AdminTokenAccount { get; set; }

            public PublicKey Club { get; set; }

            public PublicKey RewardMint { get; set; }

            public PublicKey RewardAccount { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class SetMetadataMintAccounts
        {
            public PublicKey ClubAdmin { get; set; }

            public PublicKey Club { get; set; }

            public PublicKey Mint { get; set; }
        }

        public class StartGameAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey NftTokenAccount { get; set; }

            public PublicKey NftMint { get; set; }

            public PublicKey MetadataAccount { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey Club { get; set; }

            public PublicKey Game { get; set; }

            public PublicKey RewardMint { get; set; }

            public PublicKey RewardAccount { get; set; }

            public PublicKey PlayerEscrow { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class CompleteGameAccounts
        {
            public PublicKey Signer { get; set; }

            public PublicKey PayoutAccount { get; set; }

            public PublicKey NftTokenAccount { get; set; }

            public PublicKey NftMint { get; set; }

            public PublicKey Player { get; set; }

            public PublicKey Club { get; set; }

            public PublicKey Game { get; set; }

            public PublicKey RewardMint { get; set; }

            public PublicKey RewardAccount { get; set; }

            public PublicKey PlayerEscrow { get; set; }

            public PublicKey TokenProgram { get; set; }

            public PublicKey AssociatedTokenProgram { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public static class TrainingGroundsProgram
        {
            public static Solana.Unity.Rpc.Models.TransactionInstruction AddProgramAdmin(AddProgramAdminAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ProgramAdmin, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ProgramAdminProof, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ProgramData, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(5876278395735979127UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction RemoveProgramAdmin(RemoveProgramAdminAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ProgramAdmin, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ProgramAdminProof, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ProgramData, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2581874002924417642UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction RegisterClub(RegisterClubAccounts accounts, CollectionIdentifier identifier, GameParams @params, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ProgramAdmin, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ProgramAdminProof, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Club, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ClubAdmin, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2421914087899810967UL, offset);
                offset += 8;
                offset += identifier.Serialize(_data, offset);
                offset += @params.Serialize(_data, offset);
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction EditGame(EditGameAccounts accounts, GameParams @params, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ClubAdmin, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Club, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(2359221301423144866UL, offset);
                offset += 8;
                offset += @params.Serialize(_data, offset);
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction FundGame(FundGameAccounts accounts, ulong amount, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ClubAdmin, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.SourceTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Club, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16156126827343120711UL, offset);
                offset += 8;
                _data.WriteU64(amount, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction WithdrawFunds(WithdrawFundsAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.ClubAdmin, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.AdminTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Club, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(15665806283886109937UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction SetMetadataMint(SetMetadataMintAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.ClubAdmin, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Club, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.Mint, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(4474527388172282435UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction StartGame(StartGameAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NftTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NftMint, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.MetadataAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Club, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Game, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.RewardMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PlayerEscrow, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(1077946599884992505UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }

            public static Solana.Unity.Rpc.Models.TransactionInstruction CompleteGame(CompleteGameAccounts accounts, ulong tokens, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Signer, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PayoutAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NftTokenAccount, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.NftMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Player, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Club, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Game, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardMint, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.RewardAccount, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.PlayerEscrow, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.AssociatedTokenProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(9537698836256408937UL, offset);
                offset += 8;
                _data.WriteU64(tokens, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction{Keys = keys, ProgramId = programId.KeyBytes, Data = resultData};
            }
        }
    }
}