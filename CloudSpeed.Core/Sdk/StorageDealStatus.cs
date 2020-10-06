using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CloudSpeed.Sdk
{
    public enum StorageDealStatus : byte
    {
        StorageDealUnknown = 0,

        // StorageDealProposalNotFound is a status returned in responses when the deal itself cannot
        // be located
        StorageDealProposalNotFound = 1,

        // StorageDealProposalRejected is returned by a StorageProvider when it chooses not to accept
        // a DealProposal
        StorageDealProposalRejected = 2,

        // StorageDealProposalAccepted indicates an intent to accept a storage deal proposal
        StorageDealProposalAccepted = 3,

        // StorageDealStaged means a deal has been published and data is ready to be put into a sector
        StorageDealStaged = 4,

        // StorageDealSealing means a deal is in a sector that is being sealed
        StorageDealSealing = 5,

        // StorageDealFinalizing means a deal is in a sealed sector and we're doing final
        // housekeeping before marking it active
        StorageDealFinalizing = 6,

        // StorageDealActive means a deal is in a sealed sector and the miner is proving the data
        // for the deal
        StorageDealActive = 7,

        // StorageDealExpired means a deal has passed its final epoch and is expired
        StorageDealExpired = 8,

        // StorageDealSlashed means the deal was in a sector that got slashed from failing to prove
        StorageDealSlashed = 9,

        // StorageDealRejecting means the Provider has rejected the deal, and will send a rejection response
        StorageDealRejecting = 10,

        // StorageDealFailing means something has gone wrong in a deal. Once data is cleaned up the deal will finalize on
        // StorageDealError
        StorageDealFailing = 11,

        // StorageDealFundsEnsured means we've deposited funds as necessary to create a deal, ready to move forward
        StorageDealFundsEnsured = 12,

        // StorageDealCheckForAcceptance means the client is waiting for a provider to seal and publish a deal
        StorageDealCheckForAcceptance = 13,

        // StorageDealValidating means the provider is validating that deal parameters are good for a proposal
        StorageDealValidating = 14,

        // StorageDealAcceptWait means the provider is running any custom decision logic to decide whether or not to accept the deal
        StorageDealAcceptWait = 15,

        // StorageDealStartDataTransfer means data transfer is beginning
        StorageDealStartDataTransfer = 16,

        // StorageDealTransferring means data is being sent from the client to the provider via the data transfer module
        StorageDealTransferring = 17,

        // StorageDealWaitingForData indicates either a manual transfer
        // or that the provider has not received a data transfer request from the client
        StorageDealWaitingForData = 18,

        // StorageDealVerifyData means data has been transferred and we are attempting to verify it against the PieceCID
        StorageDealVerifyData = 19,

        // StorageDealEnsureProviderFunds means that provider is making sure it has adequate funds for the deal in the StorageMarketActor
        StorageDealEnsureProviderFunds = 20,

        // StorageDealEnsureClientFunds means that client is making sure it has adequate funds for the deal in the StorageMarketActor
        StorageDealEnsureClientFunds = 21,

        // StorageDealProviderFunding means that the provider has deposited funds in the StorageMarketActor and it is waiting
        // to see the funds appear in its balance
        StorageDealProviderFunding = 22,

        // StorageDealClientFunding means that the client has deposited funds in the StorageMarketActor and it is waiting
        // to see the funds appear in its balance
        StorageDealClientFunding = 23,

        // StorageDealPublish means the deal is ready to be published on chain
        StorageDealPublish = 24,

        // StorageDealPublishing means the deal has been published but we are waiting for it to appear on chain
        StorageDealPublishing = 25,

        // StorageDealError means the deal has failed due to an error, and no further updates will occur
        StorageDealError = 26

    }
}
