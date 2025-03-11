// MTPPSCRANET, Version=1.0.0.18, Culture=neutral, PublicKeyToken=null
// MTPPSCRANET.deviceMessageTipOrCashbackEvent
public delegate void deviceMessageTipOrCashbackEvent(byte opStatus, byte mode, byte[] amount, byte[] tax, byte[] taxRate, byte[] tipOrCashback, byte reserved);
