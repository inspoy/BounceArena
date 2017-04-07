/**
 * Created by inspoy on 2017/4/7.
 */

class Battle {
    constructor() {
        this.battleId = "";
        this.users = {};
    }

}

module.exports = {
    battleId: "", // TODO:临时的战斗ID，之后要改成动态创建，这个属性要删掉
    Battle: Battle,
    battleList: {}
};
