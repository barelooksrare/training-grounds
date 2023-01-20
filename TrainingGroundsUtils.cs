using System.Text;
using System.Collections.Generic;
using Solnet;
using Solnet.Programs;
using Solnet.Programs.Abstract;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using TrainingGrounds.Accounts;
using TrainingGrounds.Program;
using TrainingGrounds.Types;

namespace TrainingGrounds.Utils
{
    public struct KeyWithBump
    {
        public PublicKey Key { get; }
        public byte Bump { get; }

        public KeyWithBump(PublicKey key, byte bump)
        {
            Key = key;
            Bump = bump;
        }
    }
    public static class TrainingGroundsUtils
    {
        public static PublicKey PROGRAM_ID = new PublicKey("SwipTb7pcUA7VH6LKeHAnx5TnCYULRU2SYVK6QiJQte");
        public static PublicKey TOKEN_PROGRAM_ID = new PublicKey("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");
        public static PublicKey TOKEN_METADATA_PROGRAM = new PublicKey("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s");
        public static PublicKey BPF_LOADER_UPGRADEABLE = new PublicKey("BPFLoaderUpgradeab1e11111111111111111111111");

        private static KeyWithBump derivePda(PublicKey programId, params object[] items)
        {
            List<byte[]> seeds = new List<byte[]>();
            foreach (var item in items)
            {
                if (item.GetType() == typeof(string)) seeds.Add(Encoding.UTF8.GetBytes((string)item));
                if (item.GetType() == typeof(PublicKey)) seeds.Add(((PublicKey)item).KeyBytes);
            }
            var success = PublicKey.TryFindProgramAddress(seeds, programId, out PublicKey key, out byte bump);
            
            return new KeyWithBump(key, bump);
        }

        private static KeyWithBump ownPda(params object[] items) => derivePda(PROGRAM_ID, items);

        public static PublicKey GetProgramDataPda() => derivePda(BPF_LOADER_UPGRADEABLE, PROGRAM_ID).Key;
        public static PublicKey GetProgramAdminProofPda(PublicKey programAdmin) => ownPda("program_admin", programAdmin).Key;
        public static PublicKey GetClubPda(PublicKey collectionKey) => ownPda("club", collectionKey).Key;
        public static PublicKey GetRewardsPda(PublicKey clubKey) => ownPda("rewards", clubKey).Key;
        public static PublicKey GetPlayerPda(PublicKey mintKey, PublicKey clubKey) => ownPda("player", mintKey, clubKey).Key;


        public static TransactionInstruction CreateAddProgramAdminInstruction(PublicKey updateAuth, PublicKey programAdmin, AddProgramAdminAccounts accounts)
        {
            accounts = accounts ?? GetAddProgramAdminAccounts(updateAuth, programAdmin);
            return TrainingGroundsProgram.AddProgramAdmin(accounts, PROGRAM_ID);
        }

        public static AddProgramAdminAccounts GetAddProgramAdminAccounts(PublicKey updateAuth, PublicKey programAdmin)
        {
            var prog = GetProgramDataPda()
                .Key;
            Console.WriteLine(prog);
            var accounts = new AddProgramAdminAccounts()
            {
                Signer = updateAuth,
                ProgramData = GetProgramDataPda(),
                ProgramAdmin = programAdmin,
                ProgramAdminProof = GetProgramAdminProofPda(programAdmin),
                SystemProgram = SystemProgram.ProgramIdKey
            };
            return accounts;
        }

        public static TransactionInstruction CreateRemoveProgramAdminInstruction(PublicKey updateAuth, PublicKey programAdmin, RemoveProgramAdminAccounts accounts)
        {
            accounts = accounts ?? GetRemoveProgramAdminAccounts(updateAuth, programAdmin);
            return TrainingGroundsProgram.RemoveProgramAdmin(accounts, PROGRAM_ID);
        }

        public static RemoveProgramAdminAccounts GetRemoveProgramAdminAccounts(PublicKey updateAuth, PublicKey programAdmin)
        {
            var accounts = new RemoveProgramAdminAccounts()
            {
                Signer = updateAuth,
                ProgramData = derivePda(PROGRAM_ID, BPF_LOADER_UPGRADEABLE).Key,
                ProgramAdmin = programAdmin,
                ProgramAdminProof = GetProgramAdminProofPda(programAdmin),
                SystemProgram = SystemProgram.ProgramIdKey
            };
            return accounts;
        }

        public static TransactionInstruction CreateRegisterClubAccountsInstruction(
            PublicKey programAdmin,
            PublicKey clubAdmin,
            PublicKey collectionId,
            PublicKey rewardMint,
            CollectionIdentifierType idType)
        {
            var accounts = GetRegisterClubAccounts(programAdmin, clubAdmin, collectionId, rewardMint);
            return TrainingGroundsProgram.RegisterClub(
                accounts,
                new CollectionIdentifier()
                {
                    Type = idType,
                    CreatorValue = idType == CollectionIdentifierType.Creator
                        ? new CreatorType()
                        {
                            Pubkey = collectionId
                        }
                        : null,
                    CollectionValue = idType == CollectionIdentifierType.Collection
                        ? new CollectionType()
                        {
                            Pubkey = collectionId
                        }
                        : null
                },
                PROGRAM_ID
            );
        }

        public static RegisterClubAccounts GetRegisterClubAccounts(PublicKey programAdmin, PublicKey clubAdmin, PublicKey collectionId, PublicKey rewardMint)
        {
            var accounts = new RegisterClubAccounts()
            {
                ProgramAdmin = programAdmin,
                ProgramAdminProof = GetProgramAdminProofPda(programAdmin),
                Club = GetClubPda(collectionId),
                ClubAdmin = clubAdmin,
                RewardMint = rewardMint,
                RewardAccount = GetRewardsPda(GetClubPda(collectionId)),
                TokenProgram = TOKEN_PROGRAM_ID,
                SystemProgram = SystemProgram.ProgramIdKey
            };
            return accounts;
        }

        public static async Task<bool> IsProgramAdminAsync(PublicKey user, IRpcClient client)
        {
            //try
            //{
            //    var res = await new TrainingGroundsClient(client, null, PROGRAM_ID).GetProgramAdminProofAsync(GetProgramAdminProofPda(user));
            //    return res.WasDeserializationSuccessful && user == res.ParsedResult.Admin;
            //} catch
            //{
            //    // There's an undefined error in deserialization / rpc client, not sure what gives.
            //    return false;
            //}
            var res = await client.GetAccountInfoAsync(GetProgramAdminProofPda(user), Commitment.Confirmed);
            if (!res.WasSuccessful || res.Result?.Value?.Data == null)
                return false;
            var resultingAccount = ProgramAdminProof.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return resultingAccount.Admin == user;
        }

        public static async Task<IEnumerable<Club>> GetActiveClubsAsync(IRpcClient client)
        {
            var res = await new TrainingGroundsClient(client, null, PROGRAM_ID).GetClubsAsync(PROGRAM_ID);
            return res.ParsedResult ?.Where(o=>o.GameParams.GameIsActive);
        }
    }
}