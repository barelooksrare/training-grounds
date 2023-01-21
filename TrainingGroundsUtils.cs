using System.Text;
using System.Collections.Generic;
using Solana.Unity;
using Solana.Unity.Programs;
using Solana.Unity.Programs.Abstract;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Core.Sockets;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using TrainingGrounds.Accounts;
using TrainingGrounds.Program;
using TrainingGrounds.Types;
using System.Threading.Tasks;
using System;
using System.Linq;

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

        public static PublicKey GetProgramDataPda() =>
            derivePda(BPF_LOADER_UPGRADEABLE, PROGRAM_ID)
                .Key;

        public static PublicKey GetProgramAdminProofPda(PublicKey programAdmin) =>
            ownPda("program_admin", programAdmin)
                .Key;

        public static PublicKey GetClubPda(PublicKey collectionKey) =>
            ownPda("club", collectionKey)
                .Key;

        public static PublicKey GetClubPda(CollectionIdentifier collectionIdentifier) =>
            GetClubPda(collectionIdentifier?.CollectionValue?.Pubkey ?? collectionIdentifier?.CreatorValue?.Pubkey);

        public static PublicKey GetRewardsPda(PublicKey clubKey) =>
            ownPda("rewards", clubKey)
                .Key;

        public static PublicKey GetPlayerPda(PublicKey mintKey, PublicKey clubKey) =>
            ownPda("player", mintKey, clubKey)
                .Key;

        public static PublicKey GetPlayerEscrowPda(PublicKey player) =>
            ownPda("escrow", player)
                .Key;

        public static PublicKey GetGamePda(PublicKey mintKey, PublicKey clubKey) =>
            ownPda("game", GetPlayerPda(mintKey, clubKey))
                .Key;


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

        public static TransactionInstruction CreateRemoveProgramAdminInstruction(
            PublicKey updateAuth,
            PublicKey programAdmin,
            RemoveProgramAdminAccounts accounts)
        {
            accounts = accounts ?? GetRemoveProgramAdminAccounts(updateAuth, programAdmin);
            return TrainingGroundsProgram.RemoveProgramAdmin(accounts, PROGRAM_ID);
        }

        public static RemoveProgramAdminAccounts GetRemoveProgramAdminAccounts(PublicKey updateAuth, PublicKey programAdmin)
        {
            var accounts = new RemoveProgramAdminAccounts()
            {
                Signer = updateAuth,
                ProgramData = derivePda(PROGRAM_ID, BPF_LOADER_UPGRADEABLE)
                    .Key,
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
            GameParams gameParams,
            CollectionIdentifierType idType,
            RegisterClubAccounts accounts = null)
        {
            accounts = accounts ?? GetRegisterClubAccounts(programAdmin, clubAdmin, collectionId, rewardMint);
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
                gameParams,
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

        public static TransactionInstruction CreateEditGameInstruction(PublicKey clubAdmin, PublicKey club, GameParams gameParams)
        {
            var accounts = new EditGameAccounts()
            {
                ClubAdmin = clubAdmin,
                Club = club
            };
            return TrainingGroundsProgram.EditGame(accounts, gameParams, PROGRAM_ID);
        }

        public static TransactionInstruction CreateSetMetadataMintInstruction(PublicKey clubAdmin, PublicKey club, PublicKey mint)
        {
            var accounts = new SetMetadataMintAccounts()
            {
                ClubAdmin = clubAdmin,
                Club = club,
                Mint = mint
            };
            return TrainingGroundsProgram.SetMetadataMint(accounts, PROGRAM_ID);
        }

        public static TransactionInstruction CreateFundGameInstruction(Club club, ulong amount, FundGameAccounts accounts = null)
        {
            // Not using math.pow here because can't be arsed with double...
            accounts = accounts ?? GetFundGameAccounts(club);
            for (var i = 0; i < club.RewardMintDecimals; i++)
            {
                amount = amount * 10;
            }

            return TrainingGroundsProgram.FundGame(accounts, amount, PROGRAM_ID);
        }

        public static FundGameAccounts GetFundGameAccounts(Club club)
        {
            return new FundGameAccounts()
            {
                ClubAdmin = club.ClubAdmin,
                Club = GetClubPda(club.CollectionIdentifier),
                RewardMint = club.RewardMint,
                RewardAccount = GetRewardsPda(GetClubPda(club.CollectionIdentifier)),
                SourceTokenAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(club.ClubAdmin, club.RewardMint),
                SystemProgram = SystemProgram.ProgramIdKey,
                TokenProgram = TOKEN_PROGRAM_ID
            };
        }

        public static TransactionInstruction CreateWithdrawFundsInstruction(Club club, WithdrawFundsAccounts accounts = null)
        {
            accounts = accounts ?? GetWithdrawFundsAccounts(club);
            return TrainingGroundsProgram.WithdrawFunds(accounts, PROGRAM_ID);
        }

        public static WithdrawFundsAccounts GetWithdrawFundsAccounts(Club club)
        {
            return new WithdrawFundsAccounts()
            {
                ClubAdmin = club.ClubAdmin,
                Club = GetClubPda(club.CollectionIdentifier),
                RewardMint = club.RewardMint,
                RewardAccount = GetRewardsPda(GetClubPda(club.CollectionIdentifier)),
                AdminTokenAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(club.ClubAdmin, club.RewardMint),
                TokenProgram = TOKEN_PROGRAM_ID,
                AssociatedTokenProgram = AssociatedTokenAccountProgram.ProgramIdKey,
                SystemProgram = SystemProgram.ProgramIdKey,
            };
        }

        public static TransactionInstruction GetStartGameInstruction(PublicKey owner, PublicKey mint, Club club, StartGameAccounts accounts = null)
        {
            accounts = accounts ?? GetStartGameAccounts(owner, mint, club);
            return TrainingGroundsProgram.StartGame(accounts, PROGRAM_ID);
        }

        public static StartGameAccounts GetStartGameAccounts(PublicKey owner, PublicKey mint, Club club)
        {
            var clubPda = GetClubPda(club.CollectionIdentifier);
            var playerPda = GetPlayerPda(mint, clubPda);
            return new StartGameAccounts()
            {
                Signer = owner,
                Club = clubPda,
                Game = GetGamePda(mint, clubPda),
                MetadataAccount = derivePda(TOKEN_METADATA_PROGRAM, "metadata", TOKEN_METADATA_PROGRAM, mint)
                    .Key,
                NftMint = mint,
                NftTokenAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(owner, mint),
                Player = playerPda,
                PlayerEscrow = GetPlayerEscrowPda(playerPda),
                RewardAccount = GetRewardsPda(clubPda),
                RewardMint = club.RewardMint,
                TokenProgram = TOKEN_PROGRAM_ID,
                SystemProgram = SystemProgram.ProgramIdKey
            };
        }

        public static TransactionInstruction GetCompleteGameInstruction(
            PublicKey owner,
            PublicKey nftMint,
            Club club,
            ulong amount,
            CompleteGameAccounts accounts = null)
        {
            accounts = accounts ?? GetCompleteGameAccounts(owner, nftMint, club);
            for (var i = 0; i < club.RewardMintDecimals; i++)
            {
                amount = amount * 10;
            }

            return TrainingGroundsProgram.CompleteGame(accounts, amount, PROGRAM_ID);
        }

        private static CompleteGameAccounts GetCompleteGameAccounts(PublicKey owner, PublicKey nftMint, Club club)
        {
            var clubPda = GetClubPda(club.CollectionIdentifier);
            var playerPda = GetPlayerPda(nftMint, clubPda);
            return new CompleteGameAccounts()
            {
                Signer = owner,
                PayoutAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(owner, club.RewardMint),
                NftTokenAccount = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(owner, nftMint),
                NftMint = nftMint,
                Player = playerPda,
                Club = GetClubPda(club.CollectionIdentifier),
                Game = GetGamePda(nftMint, GetClubPda(club.CollectionIdentifier)),
                RewardMint = club.RewardMint,
                RewardAccount = GetRewardsPda(clubPda),
                PlayerEscrow = GetPlayerEscrowPda(playerPda),
                TokenProgram = TOKEN_PROGRAM_ID,
                AssociatedTokenProgram = AssociatedTokenAccountProgram.ProgramIdKey,
                SystemProgram = SystemProgram.ProgramIdKey,
            };
        }

        public static async Task<bool> IsProgramAdminAsync(PublicKey user, IRpcClient client)
        {
            var res = await client.GetAccountInfoAsync(GetProgramAdminProofPda(user), Commitment.Confirmed);
            if (!res.WasSuccessful
             || res.Result?.Value?.Data == null)
                return false;
            var resultingAccount = ProgramAdminProof.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return resultingAccount.Admin == user;
        }

        public static async Task<List<Club>> FetchActiveClubsAsync(IRpcClient client)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>
            {
                new Solana.Unity.Rpc.Models.MemCmp
                {
                    Bytes = Club.ACCOUNT_DISCRIMINATOR_B58,
                    Offset = 0
                },
                new MemCmp
                {
                    Bytes = Convert.ToBase64String(
                        new[]
                        {
                            Convert.ToByte(true)
                        }
                    ),
                    Offset = 107
                }
            };
            var res = await client.GetProgramAccountsAsync(PROGRAM_ID, Commitment.Confirmed, memCmpList: list);
            if (!res.WasSuccessful) return new List<Club>(0);
            List<Club> resultingAccounts = new List<Club>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Club.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return resultingAccounts;
        }

        public static async Task<List<Club>> FetchAllClubsAsync(IRpcClient client)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp>
            {
                new Solana.Unity.Rpc.Models.MemCmp
                {
                    Bytes = Club.ACCOUNT_DISCRIMINATOR_B58,
                    Offset = 0
                }
            };
            var res = await client.GetProgramAccountsAsync(PROGRAM_ID, Commitment.Confirmed, memCmpList: list);
            if (!res.WasSuccessful) return new List<Club>(0);
            List<Club> resultingAccounts = new List<Club>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => Club.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return resultingAccounts;
        }

        public static async Task<Player> FetchPlayerInfoAsync(PublicKey mint, Club club, IRpcClient client)
        {
            var res = await client.GetAccountInfoAsync(GetPlayerPda(mint, GetClubPda(club.CollectionIdentifier)));
            if (!res.WasSuccessful
             || res.Result?.Value?.Data?[0] == null
             || res.Result.Value.Data[0]
                    .Length
             == 0)
                return null;
            var resultingAccount = Player.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return resultingAccount;
        }

        public static async Task<Game> FetchPlayerGameAsync(PublicKey mint, Club club, IRpcClient client)
        {
            var res = await client.GetAccountInfoAsync(GetGamePda(mint, GetClubPda(club.CollectionIdentifier)));
            if (!res.WasSuccessful
             || res.Result?.Value?.Data?[0] == null
             || res.Result.Value.Data[0]
                    .Length
             == 0)
                return null;
            var resultingAccount = Game.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return resultingAccount;

        }

    }
}