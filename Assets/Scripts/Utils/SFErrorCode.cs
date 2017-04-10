/**
 * Created on 2017/3/19 by Inspoy.
 * All rights reserved.
 */

namespace SF
{
    class SFErrorCode
    {
        public const int duplicatedLogin = 1001;            // 重复登陆
        public const int battleIdNotExist = 2001;           // 不存在的battleId
        public const int userNotLogin = 2002;               // 用户未登录
        public const int userAlreadyJoin = 2003;            // 用户已经加入了战场
        public const int userNotJoin = 2004;                // 用户未加入战场
    };
}