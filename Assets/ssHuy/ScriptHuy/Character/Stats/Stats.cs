using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class Stats quản lý chỉ số của một đối tượng (ví dụ: nhân vật, quái vật).
/// Các chỉ số bao gồm: Attack, Defense, Speed, Mana, Health.
/// </summary>
public class Stats 
{
    public event EventHandler OnStatsChanged; // Sự kiện thông báo khi có sự thay đổi chỉ số

    public static int STAT_MIN = 0;  // Giá trị tối thiểu của một chỉ số
    public static int STAT_MAX = 100; // Giá trị tối đa của một chỉ số

    public enum Type { Attack, Defense, Speed, Mana, Health }    

    private SingleStat attackStat;
    private SingleStat defenseStat;
    private SingleStat speedStat;
    private SingleStat manaStat;
    private SingleStat healthStat;

    /// Constructor khởi tạo các chỉ số với giá trị ban đầu.
    public Stats(int attackStatAmount, int defenseStatAmount, int speedStatAmount, int manaStatAmount, int healthStatAmount)
    {
        attackStat  = new SingleStat(attackStatAmount);
        defenseStat = new SingleStat(defenseStatAmount);
        speedStat   = new SingleStat(speedStatAmount);
        manaStat    = new SingleStat(manaStatAmount);
        healthStat  = new SingleStat(healthStatAmount);
    }

    /// Lấy đối tượng `SingleStat` tương ứng với loại chỉ số được yêu cầu.
    private SingleStat GetSingleStat (Type statType)
    {
        switch (statType)
        {
            default: 
            case Type.Attack:   return attackStat;
            case Type.Defense:  return defenseStat;
            case Type.Speed:    return speedStat;
            case Type.Mana:     return manaStat;
            case Type.Health:   return healthStat;
        }
    }

    /// Cập nhật giá trị của một chỉ số và kích hoạt sự kiện OnStatsChanged.
    public void SetStatAmount(Type statType, int StatAmount)
    {
        GetSingleStat(statType).SetStatAmount(StatAmount);
        if (OnStatsChanged != null) OnStatsChanged(this, EventArgs.Empty);
    }

    /// Tăng giá trị của một chỉ số lên 1 đơn vị.
    public void IncreaseStatAmount(Type statType )
    {
        SetStatAmount(statType, GetStatAmount(statType) + 1);
    }

    /// Giảm giá trị của một chỉ số xuống 1 đơn vị.
    public void DecreaseStatAmount(Type statType)
    {
        SetStatAmount(statType, GetStatAmount(statType) - 1);
    }

    /// Trả về giá trị hiện tại của một chỉ số.
    public int GetStatAmount(Type statType)
    {
        return GetSingleStat(statType).GetStatAmount();
    }

    /// Trả về giá trị của chỉ số theo dạng chuẩn hóa (0 đến 1).
    public float GetStatAmountNormalized(Type statType)
    {
        return GetSingleStat(statType).GetStatAmountNormalized();
    } 

    /// Lớp nội bộ `SingleStat` đại diện cho một chỉ số đơn lẻ.
    private class SingleStat
    {
        private int Stat; // Giá trị của chỉ số

        /// Constructor khởi tạo chỉ số với giá trị ban đầu.
        public SingleStat(int StatAmount)
        {
            SetStatAmount(StatAmount);
        }
        /// Thiết lập giá trị của chỉ số (giới hạn trong khoảng STAT_MIN đến STAT_MAX).
        public void SetStatAmount(int StatAmount)
        {
            Stat = Math.Clamp(StatAmount, STAT_MIN, STAT_MAX);
        }

        /// Lấy giá trị hiện tại của chỉ số.
        public int GetStatAmount()
        {
            return Stat;
        }

        /// Trả về giá trị chuẩn hóa của chỉ số (giá trị hiện tại chia cho giá trị tối đa).
        public float GetStatAmountNormalized()
        {
            return (float)Stat / STAT_MAX;
        } 
    }
}
