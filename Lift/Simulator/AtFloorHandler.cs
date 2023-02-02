////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   At Floor delegate
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/3/2018   RCS       Initial code.
//  
// 
//=====================================================[ Copyright ]=====================================================
// 
//  Copyright Advanced Performance
//  All Rights Reserved THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Advanced Performance
//  The copyright notice above does not evidence any actual or intended publication of such source code.
//  Some third-party source code components may have been modified from their original versions
//  by Advanced Performance. The modifications are Copyright Advanced Performance., All Rights Reserved.
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using WPFElevator.View;

namespace WPFElevator
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///  Delegate: the At Floor
    /// </summary>
    /// <param name="elevator">  The elevator.</param>
    /// <param name="floor">     The floor.</param>
    /// <param name="direction"> [optional=Direction.DOWN] The direction.</param>
    /// <returns>
    ///  The value.
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public delegate void AtFloorHandler(Elevator elevator, int floor, Direction direction = Direction.DOWN);
}
