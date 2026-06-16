import React, { useEffect, useState } from 'react';
import { PageContainer, StatisticCard } from '@ant-design/pro-components';
import { Col, Row, Card, Spin, Empty, Statistic } from 'antd';
import {
  BankOutlined,
  TeamOutlined,
  UserOutlined,
  BookOutlined,
  CheckCircleOutlined,
  MoneyCollectOutlined,
  RiseOutlined,
} from '@ant-design/icons';
import { getOrgDashboard } from '@/services/statistics';

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState<API.DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>();

  useEffect(() => {
    getOrgDashboard()
      .then(setStats)
      .catch((e) => setError(e?.message || '获取数据失败'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <PageContainer>
        <div style={{ textAlign: 'center', padding: '80px 0' }}>
          <Spin size="large" tip="加载中..." />
        </div>
      </PageContainer>
    );
  }

  if (error) {
    return (
      <PageContainer>
        <Empty description={error} />
      </PageContainer>
    );
  }

  const data = stats || {
    orgCount: 0,
    studentCount: 0,
    teacherCount: 0,
    courseCount: 0,
    todayAttendanceCount: 0,
    monthRevenue: 0,
  };

  return (
    <PageContainer>
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={6}>
          <StatisticCard
            statistic={{
              title: '机构总数',
              value: data.orgCount,
              icon: <BankOutlined style={{ fontSize: 24, color: '#1677ff' }} />,
            }}
          />
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <StatisticCard
            statistic={{
              title: '学生总数',
              value: data.studentCount,
              icon: <TeamOutlined style={{ fontSize: 24, color: '#52c41a' }} />,
            }}
          />
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <StatisticCard
            statistic={{
              title: '教师总数',
              value: data.teacherCount,
              icon: <UserOutlined style={{ fontSize: 24, color: '#faad14' }} />,
            }}
          />
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <StatisticCard
            statistic={{
              title: '课程总数',
              value: data.courseCount,
              icon: <BookOutlined style={{ fontSize: 24, color: '#722ed1' }} />,
            }}
          />
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} sm={12} lg={8}>
          <Card bordered={false}>
            <Statistic
              title="今日签到数"
              value={data.todayAttendanceCount}
              prefix={<CheckCircleOutlined style={{ color: '#52c41a' }} />}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={8}>
          <Card bordered={false}>
            <Statistic
              title="本月营收"
              value={data.monthRevenue}
              precision={2}
              prefix={<MoneyCollectOutlined style={{ color: '#f5222d' }} />}
              suffix="元"
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={8}>
          <Card bordered={false}>
            <Statistic
              title="今日活跃机构"
              value={0}
              prefix={<RiseOutlined style={{ color: '#1677ff' }} />}
              suffix="/ {data.orgCount}"
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} lg={12}>
          <Card title="平台数据趋势" bordered={false}>
            <div style={{ textAlign: 'center', color: '#999', padding: '60px 0' }}>
              图表区域 — 接入图表组件后展示
            </div>
          </Card>
        </Col>
        <Col xs={24} lg={12}>
          <Card title="近期订阅动态" bordered={false}>
            <Empty description="暂无订阅动态" />
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
};

export default Dashboard;
