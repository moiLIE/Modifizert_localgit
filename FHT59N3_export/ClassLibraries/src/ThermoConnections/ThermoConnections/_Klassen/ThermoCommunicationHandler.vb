'###################### Header #######################'
'# Firma:	Thermo Electron (Erlangen) GmbH	    	 #
'# Author: Thomas Kuschel							 #	
'#####################################################'

''' <summary>
''' This class is a summary of the DataCollector and the DataCollector_WithOutEvents, so it can be operated with events or with polling
''' Additionally it can handle all data transfers which need a wake up sign before the real data transfer
''' 
''' IMPORTANT: This class is an "Alias" for the ThermoDataCollectorA. This name is not very significant. Therefor we wanted to rename it.
''' But we cannot rename the old class itself because it is used several times.
''' </summary>
''' <remarks></remarks>
Public Class ThermoCommunicationHandler
    Inherits ThermoDataCollectorA

    ''' <summary>
    ''' Konstruktor
    ''' </summary>
    ''' <param name="MyInterface">Interface zur RS232/Ethernet</param>
    ''' <param name="Repeats">Wieviele Wiederholungen beim Timeout</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal MyInterface As ThermoIOControls.ThermoIOControl_General, ByVal Repeats As Integer)
        MyBase.New(MyInterface, Repeats)
    End Sub

End Class
