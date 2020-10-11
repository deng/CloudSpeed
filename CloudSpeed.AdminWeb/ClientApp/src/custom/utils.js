//转化url中的参数为对象
export function getQueryObject() {
  return function (a) {
    if (a == '') return {};
    var b = {};
    for (var i = 0; i < a.length; ++i) {
      var p = a[i].split('=');
      if (p.length != 2) continue;
      b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, ' '));
    }
    return b;
  }(window.location.href.indexOf('?') >= 0 ? window.location.href.split('?')[1].split('&') : window.location.search.slice(1).split('&'));
}

//Worker任务类型
export const taskTypeDict = [
  { code: 'AP', codeName: 'AddPiece' },
  { code: 'PC1', codeName: 'PreCommit1' },
  { code: 'PC2', codeName: 'PreCommit2' },
  { code: 'C1', codeName: 'Commit1' },
  { code: 'C2', codeName: 'Commit2' },
  { code: 'FIN', codeName: 'Finalize' },
  { code: 'GET', codeName: 'Fetch' },
  { code: 'UNS', codeName: 'Unseal' },
  { code: 'RD', codeName: 'ReadUnsealed' },
];

//Sector状态类型
export const sectorStateDict = [
  { code: 'Packing', codeName: 'Packing' },
  { code: 'PreCommitWait', codeName: 'PreCommitWait' },
  { code: 'PreCommit1', codeName: 'PreCommit1' },
  { code: 'PreCommit2', codeName: 'PreCommit2' },
  { code: 'PreCommitting', codeName: 'PreCommitting' },
  { code: 'WaitSeed', codeName: 'WaitSeed' },
  { code: 'Committing', codeName: 'Committing' },
  { code: 'CommitWait', codeName: 'CommitWait' },
  { code: 'FinalizeSector', codeName: 'FinalizeSector' },
  { code: 'Proving', codeName: 'Proving' },
  { code: 'SealPreCommit1Failed', codeName: 'SealPreCommit1Failed' },
  { code: 'SealPreCommit2Failed', codeName: 'SealPreCommit2Failed' },
  { code: 'PreCommitFailed', codeName: 'PreCommitFailed' },
  { code: 'ComputeProofFailed', codeName: 'ComputeProofFailed' },
  { code: 'CommitFailed', codeName: 'CommitFailed' },
  { code: 'FinalizeFailed', codeName: 'FinalizeFailed' },
  { code: 'FailedUnrecoverable', codeName: 'FailedUnrecoverable' },
  { code: 'DealsExpired', codeName: 'DealsExpired' },
  { code: 'Removing', codeName: 'Removing' },
  { code: 'Empty', codeName: 'Empty' },
];

//Sector状态类型
export const mergeSectorStateDict = function(states){
  if(states){
    return sectorStateDict.map(ss => {
      if(states[ss.code]){
        return {
          ...ss, codeName: `${ss.codeName} (${states[ss.code]})`
        };
      } else {
        return ss;
      }
    });
  }
  return sectorStateDict;
}

//updatingStateDict
export const updatingStateDict = [
  { code: 'FailedUnrecoverable', codeName: 'FailedUnrecoverable' },
];