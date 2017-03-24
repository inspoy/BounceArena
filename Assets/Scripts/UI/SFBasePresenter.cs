/**
 * Created on 2017/03/24 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public interface ISFBasePresenter
    {
        void initWithView(SFBaseView view);
        void onViewRemoved();
    }
}