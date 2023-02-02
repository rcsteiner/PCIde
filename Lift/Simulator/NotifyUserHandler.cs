////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Description:   Notification passanger handler
// 
// 
//  Author:        Robert C Steiner
// 
//======================================================[ History ]======================================================
// 
//  Date        Who      What
//  ----------- ------   ----------------------------------------------
//  4/8/2018   RCS       Initial code.
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
    ///  Delegate: the Notify Passanger Handler
    /// </summary>
    /// <param name="passanger"> The passanger.</param>
    /// <param name="elevator">  The elevator.</param>
    /// <param name="floorNum">  The floor number.</param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public delegate void NotifyUserHandler(Passanger passanger, Elevator elevator, int floorNum);
}